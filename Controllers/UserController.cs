using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameFinder.ApiRelated;
using VideoGameFinder.Controllers.ControllerDtos;
using VideoGameFinder.Model;
using RegisterRequest = VideoGameFinder.Controllers.ControllerDtos.RegisterRequest;

namespace VideoGameFinder.Controllers;

[ApiController]
[AllowAnonymous]
[Route("Users")]
public class UserController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    private readonly IConfiguration _configuration;

    public UserController(DatabaseContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _dbContext.Users.AnyAsync(x => x.Username == request.Username))
        {
            return Ok(new ApiErrorDto
            {
                Message = "User already exists.",
                Attempted = request.Username,
                Timestamp = default,
                Suggestions = "Please choose a different username."
            });
        }

        var user = new User
        {
            UserGuid = default,
            Username = request.Username,
            Password = request.Password,
            Role = "User"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return Ok("New user created.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        if (EndpointMethods.LoginCheck(_dbContext, username, password))
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x =>
                x.Username == username && x.Password == password);
            var token = EndpointMethods.GenerateNewJwtToken(_configuration, user);
            return Ok($"Login successful. User token: {token}");
        }

        return Ok(new ApiErrorDto
        {
            Message = "Invalid username or password.",
            Attempted = $"Username: {username}, password: {password}",
            Timestamp = default,
            Suggestions = "Please ensure user exists and password is correct."
        });
    }
}
    
    