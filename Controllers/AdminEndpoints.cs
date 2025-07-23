using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameFinder.Model;

namespace VideoGameFinder.Controllers;

[ApiController]
[Route("api/AdminEndpoints")]
public class AdminEndpoints : ControllerBase
{
    private readonly VideoGameContext _dbcontext;

    public AdminEndpoints(VideoGameContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    
    [HttpDelete("DELETEALL")]
    public async Task<ActionResult> DeleteAll()
    {
        var games = await _dbcontext.VideoGames.ToListAsync();

        if (games.Count == 0)
        {
            return Ok("No games exist");
        }
        foreach (var game in games)
        {
            _dbcontext.VideoGames.Remove(game);
        }
        
        await _dbcontext.SaveChangesAsync();
        return NoContent();
    }
}