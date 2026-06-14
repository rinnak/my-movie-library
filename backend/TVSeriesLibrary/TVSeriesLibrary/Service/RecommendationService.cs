using Microsoft.EntityFrameworkCore;
using TVSeriesLibrary.Data;
using TVSeriesLibrary.Models;

namespace TVSeriesLibrary.Service;

public class RecommendationService : IRecommendationService
{
    private readonly ApplicationDb _context;
    public readonly IPoiskKinoService _poiskKinoService;
    private const double MinRatingThreshold = 7.0;

    public RecommendationService(ApplicationDb context, IPoiskKinoService poiskKinoService)
    {
        _context = context;
        _poiskKinoService = poiskKinoService;
    }

    public async Task<List<MovieCard>> GetFeedAsync(
        int? userId,
        int page = 1,
        int limit = 14 , string? genre = null, string? sortBy = "none", 
        string? yearRange = "all",
        string? favIds = null, 
        string? watchedIds = null)
    {
        if ((!string.IsNullOrWhiteSpace(genre) && genre.ToLower() != "all") || 
            (!string.IsNullOrEmpty(yearRange) && yearRange.ToLower() != "all") || 
            (!string.IsNullOrEmpty(sortBy) && sortBy.ToLower() != "none"))
        {
            return await _poiskKinoService.GetMoviesWithFiltersAsync(genre, yearRange, sortBy, page, limit);
        }
        List<MovieCard> poolMovies = new();
        int poolLimit = 100;
        
        if (!string.IsNullOrWhiteSpace(genre) && genre.ToLower() != "all")
        {
            poolMovies  = await _poiskKinoService.GetMoviesByGenreAsync(genre, page, poolLimit);
        }
        if (userId == null)
        {
            if (!string.IsNullOrEmpty(favIds) || !string.IsNullOrEmpty(watchedIds))
            {
                poolMovies = await GetGuestPersonalFeedAsync(favIds, watchedIds, page, poolLimit);
            }
            else
            {
                return await GetGuestFeedAsync(page, limit);
            }
        }
        else
        {
            poolMovies = await GetPersonalFeedAsync(userId.Value, page, poolLimit);
        }
        
        poolMovies = poolMovies.Where(m => !m.Rating.HasValue || m.Rating.Value >= MinRatingThreshold).ToList();

        if (!string.IsNullOrEmpty(yearRange) && yearRange.ToLower() != "all")
        {
            var parts = yearRange.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int startYear) &&
                int.TryParse(parts[1].Trim(), out int endYear))
            {
                poolMovies = poolMovies.Where(m => m.Year >= startYear && m.Year <= endYear).ToList();
            }
        }

        if (!string.IsNullOrEmpty(sortBy) && sortBy.ToLower() != "none")
        {
            if (sortBy == "rating")
            {
                poolMovies = poolMovies.OrderByDescending(m => m.Rating ?? 0).ToList();
            }
            else if (sortBy.ToLower() == "year")
            {
                poolMovies = poolMovies.OrderByDescending(m => m.Year ?? 0).ToList();
            }
        }

        return poolMovies.Take(limit).ToList();
    }
    
    public async Task<List<MovieCard>> GetPersonalFeedAsync(int userId, int page = 1, int limit = 14)
    {
        var userMovies = await _context.UserMovies
            .Where(u => u.UserId == userId)
            .ToListAsync();

        var excludedMovieIds = userMovies.Select(u => u.MovieId).ToHashSet();
        var feedMovies = new List<MovieCard>();
        var addedMovieIds = new HashSet<int>();
        
        var seed = userId + DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
        var random = new Random(seed);

        var favoriteGenres = userMovies
            .Where(u => u.IsFavorite || (u.IsWatched &&
                                         u.UserRating.HasValue && u.UserRating >= 7))
            .Where(u => !string.IsNullOrEmpty(u.Genres))
            .SelectMany(u => u.Genres.Split(','))
            .GroupBy(genre => genre.Trim().ToLower())
            .Select(g => new { Genre = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToList();

        if (favoriteGenres.Count < 2)
        {
            var topMovies = await _poiskKinoService.GetTopMoviesAsync(page, 50);
            AddUniqueMovies(topMovies, feedMovies, addedMovieIds, excludedMovieIds, limit);
            
            return feedMovies.OrderBy(_ => random.Next()).ToList();
        }

        var topGenre = favoriteGenres[0].Genre;
        var secondGenre = favoriteGenres[1].Genre;

        int topGenreLimit = (int)(limit * 0.6);
        int secondGenreLimit = (int)(limit * 0.3);
        int randomLimit = limit - topGenreLimit - secondGenreLimit;

        
        var topGenreTask = _poiskKinoService.GetMoviesByGenreAsync(topGenre, page, topGenreLimit * 2);
        var secondGenreTask = _poiskKinoService.GetMoviesByGenreAsync(secondGenre, page, secondGenreLimit * 2);
        
        await Task.WhenAll(topGenreTask, secondGenreTask);
        
        AddUniqueMovies(topGenreTask.Result, feedMovies, addedMovieIds, excludedMovieIds, topGenreLimit);
        
        AddUniqueMovies(secondGenreTask.Result, feedMovies,  addedMovieIds, excludedMovieIds, secondGenreLimit);

        if (feedMovies.Count < limit)
        {
            var additionalMovies = await _poiskKinoService.GetTopMoviesAsync(page, 50);
            AddUniqueMovies(additionalMovies, feedMovies, addedMovieIds, excludedMovieIds, limit);
        }

        return feedMovies.OrderBy(_ => random.Next()).ToList();
        
    }

    private void AddUniqueMovies(IEnumerable<MovieCard> source, List<MovieCard> destination, HashSet<int> addedIds,
        HashSet<int> excludedIds, int maxCount)
    {
        int addedCount = 0;
        foreach (var movie in source)
        {
            if (addedCount >= maxCount) break;
            if (movie.Rating.HasValue && movie.Rating.Value < MinRatingThreshold) continue;
            if (excludedIds.Contains(movie.Id)) continue;
            if (addedIds.Contains(movie.Id)) continue;
            
            destination.Add(movie);
            addedIds.Add(movie.Id);

            addedCount++;
        }
    }

    private async Task<List<MovieCard>> GetGuestFeedAsync(
        int page,
        int limit)
    {
        return await _poiskKinoService.GetTopMoviesAsync(page, limit);
    }

    private async Task<List<MovieCard>> GetGuestPersonalFeedAsync(string? favIds, string? watchedIds, int page, int limit)
    {
        var favoriteIds = string.IsNullOrEmpty(favIds)
            ? new List<int>()
            : favIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

        var watchedMovieIds = string.IsNullOrEmpty(watchedIds)
            ? new List<int>()
            : watchedIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

        var excludedMovieIds = favoriteIds.Concat(watchedMovieIds).ToHashSet();

        var feedMovies = new List<List<MovieCard>>();
        var resultMovies = new List<MovieCard>();
        var addedMovieIds = new HashSet<int>();
        var random = new Random();
        
        int limitWithBuffer = limit + excludedMovieIds.Count;
        var topMovies = await _poiskKinoService.GetTopMoviesAsync(page, limitWithBuffer);
        
        AddUniqueMovies(topMovies, resultMovies, addedMovieIds, excludedMovieIds, limit);
        
        return resultMovies.OrderBy(_ => random.Next()).ToList();
    }

}