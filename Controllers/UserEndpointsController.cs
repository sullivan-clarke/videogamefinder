using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameFinder.Controllers.ControllerDtos;
using VideoGameFinder.DbSeed;
using VideoGameFinder.Model;

namespace VideoGameFinder.Controllers;

[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/UserEndpoints")]
public class UserEndpointsController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public UserEndpointsController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpDelete("Wipe-Db")]
    public async Task<ActionResult> DeleteAll()
    {
        var games = await _dbContext.VideoGames.ToListAsync();

        if (games.Count == 0)
        {
            return Ok(new ApiErrorDto
            {
                Message = "All video game records were already deleted.",
                Attempted = "api/AdminEndpoints/Wipe-Db",
                Timestamp = default,
                Suggestions = "Please repopulate the database via the endpoint or restart the container."
            });
        }
        foreach (var game in games)
        {
            _dbContext.VideoGames.Remove(game);
        }
        
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPost("add-video-game")]
    public async Task<ActionResult> Post(VideoGame videoGame)
    {
        await _dbContext.VideoGames.AddAsync(videoGame);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{guid}")]
    public async Task<ActionResult> Delete(Guid guid)
    {
        var gameTitle = await _dbContext.VideoGames.FirstOrDefaultAsync(x => x.GameGuid == guid);
    
        if (gameTitle is null)
        {
            return  Ok(new ApiErrorDto
            {
                Message = "Video Game not found",
                Attempted = guid.ToString(),
                Suggestions = "Please check GUID to ensure it is correct."
            });
        }
        _dbContext.VideoGames.Remove(gameTitle);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("Refresh-Db")]
    public async Task<ActionResult> RefreshDb()
    {
        var games = await _dbContext.VideoGames.ToListAsync();
        
        if (games.Count == 0)
        {
            return Ok(new ApiErrorDto
            {
                Message = "No games exist in the database.",
                Attempted = "api/AdminEndpoints/Refresh-Db",
                Timestamp = default,
                Suggestions = "Please repopulate the database via the endpoint or restart the container."
            });
        }
        
        _dbContext.VideoGames.RemoveRange(games);

        var refreshedGames = games.Select(game => new VideoGame
        {
            GameGuid = Guid.NewGuid(),
            Game = game.Game,
            CopiesSold = game.CopiesSold,
            ReleaseDate = game.ReleaseDate,
            Genre = game.Genre,
            Developer = game.Developer,
            Publisher = game.Publisher
        });
        
        await _dbContext.VideoGames.AddRangeAsync(refreshedGames);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("Repopulate-Db")]
    public async Task<ActionResult> RepopulateDb()
    {
        var games = await _dbContext.VideoGames.ToListAsync();
        if (games.Count != 0)
            return Ok(new ApiErrorDto
            {
                Message = "Games already exist in database.",
                Attempted = "api/AdminEndpoints/Repopulate-Db",
                Timestamp = default,
                Suggestions = "Ensure all games are deleted before attempting to repopulate the database."
            });
        await DbInitializer.SeedDbAsync(_dbContext);
        return NoContent();
    
    }
}