using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVSeriesLibrary.Models;
using TVSeriesLibrary.Service;

namespace TVSeriesLibrary.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;
    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }
    
    private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost("favorites/{movieId}")]
    public async Task<IActionResult> AddFavorite(int movieId)
    {
        try
        {
            await _libraryService
                .AddFavoriteAsync(CurrentUserId, movieId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }

    [HttpDelete("favorites/{movieId}")]
    public async Task<IActionResult> RemoveFavorite(int movieId)
    {
        try
        {
            await _libraryService
                .RemoveFavoriteAsync(CurrentUserId, movieId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                Message = e.Message
            });
        }
    }

    [HttpGet("favorites")]
    public async Task<ActionResult<List<MovieCard>>> GetFavorites()
    {
        var movies = await _libraryService.GetFavoriteMoviesAsync(CurrentUserId);
        return Ok(movies);
    }

    [HttpPost("watched/{movieId}")]
    public async Task<IActionResult> AddWatched(int movieId, [FromBody] int rating)
    {
        try
        {
            if (rating < 1 || rating > 10)
            {
                return BadRequest(new { Message = "Оценка должна быть от 1 до 10" });
            }

            await _libraryService.AddWatchedMovieAsync(CurrentUserId, movieId, rating);
            return Ok(new { Message = "Сериал добавлен в просмотренные с оценкой" });
        }
        catch (Exception e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }

    [HttpDelete("watched/{movieId}")]
    public async Task<IActionResult> RemoveWatched(int movieId)
    {
        try
        {
            await _libraryService.RemoveWatchedMovieAsync(CurrentUserId, movieId);
            return Ok(new { Message = "Сериал удален из просмотренных" });
        }
        catch (Exception e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }

    [HttpGet("watched")]
    public async Task<ActionResult<List<WatchedMovieDto>>> GetWatched()
    {
        var movies = await _libraryService.GetWatchedMoviesAsync(CurrentUserId);
        return Ok(movies);
    }

    [HttpPut("watched/{movieId}/rating")]
    public async Task<IActionResult> UpdateWatchedRating(int movieId,[FromBody] int userRating)
    {
        try
        {
            if (userRating < 1 || userRating > 10)
            {
                return BadRequest(new { Message = "Rating must be from 1 to 10" });
            }

            await _libraryService.UpdateRatingAsync(CurrentUserId, movieId, userRating);
            return Ok(new { Message = "Оценка обновлена" });
        }
        catch (Exception e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }
}