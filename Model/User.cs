using System.ComponentModel.DataAnnotations;

namespace VideoGameFinder.Model;

public class User
{
    [Key]
    public Guid UserGuid { get; set; } = Guid.NewGuid();

    public required string Username { get; set; }

    public required string Password { get; set; }

    public required string Role { get; set; }
    
    //Add salt and hash later?
}