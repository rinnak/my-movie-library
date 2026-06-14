using Microsoft.EntityFrameworkCore;
using TVSeriesLibrary.Data;
using TVSeriesLibrary.Models;

namespace TVSeriesLibrary.Service;

public class LibraryService : ILibraryService
{

    private readonly ApplicationDb _context;
    private readonly IPoiskKinoService _poiskKinoService;

    public LibraryService(ApplicationDb context, IPoiskKinoService poiskKinoService)
    {
        _context = context;
        _poiskKinoService = poiskKinoService;
    }

    private async Task<UserMovie> GetOrCreateUserMovieAsync(int userId, int movieId)
    {
        var alredyExists = await _context.UserMovies.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);
        if (alredyExists != null)
        {
            return alredyExists;
        }
        
        var movie = await _poiskKinoService.GetMovieByIdAsync(movieId);
        if (movie == null)
        {
            throw new Exception("Movie not found");
        }
        string genresString = movie.Genres != null ? string.Join(",", movie.Genres) : string.Empty;
        return new UserMovie
        {
            UserId = userId,
            MovieId = movieId,
            
            MovieName = movie.Name,
            PosterUrl = movie.PosterUrl,
            Rating = movie.Rating,
            Year = movie.Year,
            Genres = genresString
        };
    }
    

    public async Task AddFavoriteAsync(int userId, int movieId)
    {
        var userMovie = await GetOrCreateUserMovieAsync(userId, movieId);
        userMovie.IsFavorite = true;
        
        if (userMovie.Id == 0) _context.UserMovies.Add(userMovie);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFavoriteAsync(int userId, int movieId)
    {
        var userMovie = await _context.UserMovies.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);
        if (userMovie == null)
        {
            return;
        }
        userMovie.IsFavorite = false;
        
        if (!userMovie.IsFavorite && !userMovie.IsWatched) _context.UserMovies.Remove(userMovie);
        await _context.SaveChangesAsync();
    }

    public async Task<List<MovieCard>> GetFavoriteMoviesAsync(int userId)
    {
        return await _context.UserMovies
            .Where(x => x.UserId == userId && x.IsFavorite)
            .Select(x => new MovieCard
            {
                Id = x.MovieId,
                Name = x.MovieName,
                PosterUrl = x.PosterUrl,
                Rating = x.Rating,
                Year = x.Year
            }).ToListAsync();
    }

    public async Task AddWatchedMovieAsync(int userId, int movieId, int userRating)
    {
        var userMovie = await GetOrCreateUserMovieAsync(userId, movieId);
        userMovie.IsWatched = true;
        userMovie.UserRating = userRating;
        
        if (userMovie.Id == 0) _context.UserMovies.Add(userMovie);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveWatchedMovieAsync(int userId, int movieId)
    {
        var userMovie = await _context.UserMovies.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);
        if (userMovie == null) return;
        
        userMovie.IsWatched = false;
        userMovie.UserRating = null;
        
        if (!userMovie.IsWatched && !userMovie.IsFavorite) _context.UserMovies.Remove(userMovie);
        
        await _context.SaveChangesAsync();
    }

    public async Task<List<WatchedMovieDto>> GetWatchedMoviesAsync(int userId)
    {
        return await _context.UserMovies
            .Where(x => x.UserId == userId && x.IsWatched)
            .Select(x => new WatchedMovieDto
            {
                MovieId = x.MovieId,
                MovieName = x.MovieName,
                PosterUrl = x.PosterUrl,
                Rating = x.Rating,
                Year = x.Year,
                UserRating = x.UserRating ?? 0
            }).ToListAsync();
    }

    public async Task UpdateRatingAsync(int userId, int movieId, int userRating)
    {
        if (userRating < 1 || userRating > 10)
        {
            throw new Exception("Rating must be from 1 to 10");
        }
        var userMovie = await _context.UserMovies.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);
        if (userMovie == null)
        {
            throw new Exception("Movie not found");
        }

        if (!userMovie.IsWatched)
        {
            throw new Exception("Movie is not watched");
        }
        
        userMovie.Rating = userRating;
        await _context.SaveChangesAsync();
    }
}