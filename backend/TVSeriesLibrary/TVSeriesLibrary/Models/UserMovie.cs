namespace TVSeriesLibrary.Models;

public class UserMovie
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MovieId { get; set; }
    
    public string MovieName { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public double? Rating { get; set; }
    public int? Year { get; set; }
    public string Genres { get; set; } = string.Empty;
    
    public bool IsFavorite { get; set; } = false;
    public bool IsWatched { get; set; } = false;
    
    public int? UserRating { get; set; }
}