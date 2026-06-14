using TVSeriesLibrary.Models;

namespace TVSeriesLibrary.Service;

public interface ILibraryService
{
    Task AddFavoriteAsync(int userId, int movieId);
    Task RemoveFavoriteAsync(int userId, int movieId);
    Task<List<MovieCard>> GetFavoriteMoviesAsync(int userId);
    
    Task AddWatchedMovieAsync(int userId, int movieId, int userRating);
    Task RemoveWatchedMovieAsync(int userId, int movieId);
    Task<List<WatchedMovieDto>> GetWatchedMoviesAsync(int userId);
    Task UpdateRatingAsync(int userId, int movieId, int userRating);
}