using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVSeriesLibrary.Models;

namespace TVSeriesLibrary.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationController(IRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }
    
    
    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed(int page = 1, int limit = 14, string? genre = null, string? sortBy = "none", string? yearRange = "all", string? favIds = null, string? watchedIds = null)
    {
        int? userId = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
                userId = int.Parse(claim.Value);
        }

        var feed = await _recommendationService.GetFeedAsync(userId, page, limit, genre, sortBy, yearRange, favIds, watchedIds);

        return Ok(feed);
    }
}