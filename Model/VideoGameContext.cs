using Microsoft.EntityFrameworkCore;

namespace VideoGameFinder.Model;

public class VideoGameContext : DbContext
{
    public DbSet<VideoGame> VideoGames { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {

        options.UseNpgsql("host=db;port=5432;database=videogamedatabase;username=sullivan;password=password");
    }
}

