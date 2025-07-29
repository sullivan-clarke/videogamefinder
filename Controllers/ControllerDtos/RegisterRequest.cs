namespace VideoGameFinder.Controllers.ControllerDtos;

public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}