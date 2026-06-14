using Microsoft.AspNetCore.Mvc;
using TVSeriesLibrary.Service;

namespace TVSeriesLibrary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IPoiskKinoService _poiskKinoService;

    public MoviesController(IPoiskKinoService poiskKinoService)
    {
        _poiskKinoService = poiskKinoService;
    }

    
    [HttpGet("hero-list")]
    public async Task<IActionResult> GetHeroListAsync()
    {
        var result = await _poiskKinoService.GetThreeHeroMoviesAsync();
        if (result == null || result.Count == 0) return NotFound();
        return Ok(result);
    }
    
    [HttpGet("random")]
    public async Task<IActionResult> GetRandomMovie()
    {
        //IActionResult универсальный интерфейс для результата работы контроллера. Возвращает какой-нибудь HTTP-ответ
        var result = await _poiskKinoService.GetRandomMovieAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMovieByIdAsync(int id)
    {
        var result = await _poiskKinoService.GetMovieByIdAsync(id);
        if (result == null) return NotFound($"Movie with id {id} not found");
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchMovies([FromQuery] string query)
    {
        var result = await _poiskKinoService.SearchMovieAsync(query);
        
        if (result == null) return NotFound($"Movie with query {query} not found");
        return Ok(result);
    }
    
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }

}