namespace TVSeriesLibrary.Models;

public class User
{
    public int Id { get; set; }
    public string Email {get; set;} = "";
    public string PasswordHash {get; set;} = "";
    public bool IsEmailConfirmed {get; set;} = false;
    public string? EmailConfiramtionToken {get; set;}
    public List<UserMovie> LibraryMovies { get; set; } = [];
}