namespace TVSeriesLibrary.Models;

public class SearchMovieResponse
{
    public List<MovieCard> Docs { get; set; } = new();
    
    public int Page  { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int Pages { get; set; }
}