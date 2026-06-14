namespace TVSeriesLibrary.Models;

public class WatchedMovieDto
{
    public int MovieId { get; set; }
    public string MovieName { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public double? Rating { get; set; }
    public int? Year { get; set; }
    public int UserRating { get; set; }
}