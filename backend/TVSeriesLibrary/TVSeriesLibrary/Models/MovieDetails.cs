namespace TVSeriesLibrary.Models;

public class MovieDetails
{
    public int? Id { get; set; } 
    public string? Name { get; set; } 
    public string? Description { get; set; } 
    public int? Year { get; set; } 
    public double? Rating { get; set; } 
    public string? PosterUrl { get; set; } 
    public List<string> Genres { get; set; } = []; 
    public List<string> Countries { get; set; } = []; 
    public List<PersonInMovieDto> Persons { get; set; } = [];
}