using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameFinder.DbSeed;
using VideoGameFinder.Model;
using static FuzzySharp.Fuzz;

namespace VideoGameFinder.Controllers;

[ApiController]
[Route("api/videogamefinder")]
public class VideoGameController : ControllerBase
{
    private readonly VideoGameContext _dbcontext;

    public VideoGameController(VideoGameContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    [HttpGet("all")]
    public async Task<ActionResult<VideoGame>> Get()
    {
        var games = await _dbcontext.VideoGames.ToListAsync();

        if (games.Count == 0)
        {
            return Ok(new ApiErrorDto
            {
                Message = "There are no video games in the database currently.",
                Attempted = "/api/videogamefinder/all",
                Suggestions = "Add video games to the database or restart the container to repopulate the database."
            });
        }

        return Ok(games);
    }

    [HttpGet("{game}/game")]
    public async Task<ActionResult<VideoGame>> Get(string game)
    {
        var gameTitle = await _dbcontext.VideoGames.FirstOrDefaultAsync(x => x.Game.ToLower() == game.ToLower());
        var games = await _dbcontext.VideoGames.ToListAsync();

        if (gameTitle is null)
        {
            var possibleGames = games.Where(x => PartialRatio(x.Game.ToLower(), game.ToLower()) > 80).ToList();
            if (!possibleGames.Any())
            {
                return Ok(new ApiErrorDto
                {
                    Message = "Video Game not found",
                    Attempted = game,
                    Suggestions = "Please check spelling and available list of games"
                });

            }

            return Ok(new ApiErrorDto
            {
                Message = "Video Game not found",
                Attempted = game,
                Suggestions = "Did you mean one of these games: " +
                              string.Join(" | ", possibleGames.Select(x => x.Game))
            });

        }
        return Ok(gameTitle);
    }


[HttpGet("{guid}")]
    public async Task<ActionResult<VideoGame>> Get(Guid guid)
    {
        var gameTitle = await _dbcontext.VideoGames.FirstOrDefaultAsync(x => x.GameGuid == guid);
        
        if (gameTitle is null)
        {
            return Ok(new ApiErrorDto
            {
                Message = "Video Game not found",
                Attempted = guid.ToString(),
                Suggestions = "Please check GUID to ensure it is correct"
            });
        }
        
        return Ok(gameTitle);
    }

    [HttpGet("hightest-sales")]
    public async Task<ActionResult<VideoGame>> HightestSales()
    {
        var games = _dbcontext.VideoGames.ToList();
        var mostSales = ApiRelated.EndpointMethods.CopiesSoldtoInt(games);
        var gameTitle = await _dbcontext.VideoGames.FirstOrDefaultAsync(x => x.CopiesSold == mostSales);
        
        return Ok(gameTitle);
    }
    
    [HttpPost("add-video-game")]
    public async Task<ActionResult> Post(VideoGame videoGame)
    {
        await _dbcontext.VideoGames.AddAsync(videoGame);
        await _dbcontext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{guid}")]
    public async Task<ActionResult> Delete(Guid guid)
    {
        var gameTitle = await _dbcontext.VideoGames.FirstOrDefaultAsync(x => x.GameGuid == guid);
        
        if (gameTitle is null)
        {
            return  Ok(new ApiErrorDto
            {
                Message = "Video Game not found",
                Attempted = guid.ToString(),
                Suggestions = "Please check GUID to ensure it is correct"
            });
        }
        _dbcontext.VideoGames.Remove(gameTitle);
        await _dbcontext.SaveChangesAsync();
        return NoContent();
    }

}

public class ApiErrorDto
{
    public string Message { get; set; } = "";
    public string Attempted { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Suggestions { get; set; } = "";
}

//Add other methods here, likely going to use LINQ and lambdas