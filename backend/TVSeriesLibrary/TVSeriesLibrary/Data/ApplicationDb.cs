using Microsoft.EntityFrameworkCore;
using TVSeriesLibrary.Models;

namespace TVSeriesLibrary.Data;

public class ApplicationDb : DbContext
{
    public ApplicationDb(DbContextOptions<ApplicationDb> options)
        : base(options)
    {
        
    }
    public DbSet<User> Users {get; set;}
    public DbSet<UserMovie> UserMovies { get; set; }
}