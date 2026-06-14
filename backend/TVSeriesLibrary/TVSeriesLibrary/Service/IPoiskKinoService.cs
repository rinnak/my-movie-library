using TVSeriesLibrary.Models;

namespace TVSeriesLibrary.Service;

public interface IPoiskKinoService
{
    Task<MovieCard?> GetRandomMovieAsync();

    Task<MovieDetails?> GetMovieByIdAsync(int id);

    Task<SearchMovieResponse?> SearchMovieAsync(string query);
    
    Task<List<MovieCard>> GetMoviesByGenreAsync(string genre, int page, int limit);

    Task<MovieDetails> GetHeroMovieAsync();
    
    Task<List<MovieDetails?>> GetThreeHeroMoviesAsync();

    Task<List<MovieCard>> GetTopMoviesAsync(int page, int limit);

    Task<List<MovieCard>> GetMoviesWithFiltersAsync(
        string? genre,
        string? yearRange,
        string? sortBy,
        int page,
        int limit);
}