using TVSeriesLibrary.Models;

public interface IRecommendationService
{
    Task<List<MovieCard>> GetFeedAsync(
        int? userId,
        int page = 1,
        int limit = 14 , string? genre = null, string? sortBy = "none", 
        string? yearRange = "all",
        string? favIds = null, 
        string? watchedIds = null);
    
    Task<List<MovieCard>> GetPersonalFeedAsync(
        int userId,
        int page = 1,
        int limit = 14);
}