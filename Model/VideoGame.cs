using System.ComponentModel.DataAnnotations;

namespace VideoGameFinder.Model;

public class VideoGame
{
    [Key] //Key has to be used here in order to tell the program that this is meant to be a table that will be update. "Keyless" = readonly 
    public Guid GameGuid { get; set; } = Guid.NewGuid();
    public required string Game { get; set; }
    public required string CopiesSold { get; set; }
    public required string ReleaseDate { get; set; }
    public required string Genre { get; set; }
    public required string Developer { get; set; }
    public required string Publisher { get; set; }
    
    
}