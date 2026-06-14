namespace TVSeriesLibrary.Models;

public class MovieApi
{
    public int Id { get; set; } 
    public string? Name { get; set; } 
    public string? Description { get; set; } 
    public int? Year { get; set; } 
    public List<ItemNameDto> Countries { get; set; } = [];
    public RatingDTO? Rating { get; set; } 
    public ShortImageDTO? Poster { get; set; } 
    public List<ItemNameDto>? Genres { get; set; } 
    public List<PersonInMovieDto>? Persons { get; set; }
}