namespace TVSeriesLibrary.Models;

public class MovieCard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PosterUrl { get; set; }
    public double? Rating { get; set; }
    public int? Year { get; set; }
    public string? Genres { get; set; }
}