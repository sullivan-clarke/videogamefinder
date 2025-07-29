using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameFinder.Controllers.ControllerDtos;
using VideoGameFinder.Model;

namespace VideoGameFinder.Controllers;

[ApiController]
[Route("Admin")]
public class AdminController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    private readonly IConfiguration _configuration;

    public AdminController(IConfiguration configuration, DatabaseContext dbContext)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult Login([FromBody] LoginModel login)
    {
        if (login.Username == "admin" && login.Password == "password123")
        {
            var adminUser = new User
            {
                UserGuid = default,
                Username = "admin",
                Password = "password123",
                Role = "Admin"
            };
            var token = ApiRelated.EndpointMethods.GenerateNewJwtToken(_configuration, adminUser);
            return Ok($"Here is your admin token: {token}");
        }

        return Unauthorized("Login failed. Please check your username and password.");
    }
    [HttpGet("get-all-users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAllUsers()
    {
        var users = await _dbContext.Users.ToListAsync();
        if (users.Count == 0)
        {
            return Ok(new ApiErrorDto
            {
                Message = "No users found.",
                Attempted = "Admin/Get-all-users",
                Timestamp = default,
                Suggestions = "Add users to the database"
            });
        }
        return Ok(users);
    }

    [HttpDelete("delete-all-users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteAllUsers()
    {
        var users = await _dbContext.Users.ToListAsync();

        if (users.Count == 0)
        {
            return Ok(new ApiErrorDto
            {
                Message = "No users exist.",
                Attempted = "Admin/delete-all-users",
                Timestamp = default,
                Suggestions = "Ensure users exist."
            });
        }

        _dbContext.Users.RemoveRange(users);
        await _dbContext.SaveChangesAsync();
        return Ok("Users successfully deleted.");
    }
    
    [HttpDelete("delete-user/{guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(Guid guid)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserGuid == guid);
        if (user is null)
        {
            return Ok(new ApiErrorDto
            {
                Message = "User does not exist.",
                Attempted = guid.ToString(),
                Timestamp = default,
                Suggestions = "Please be sure the user exists."
            });
        }
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        
        return Ok("User deleted successfully.");
    }

    [HttpGet("get-user/{guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> GetUser(Guid guid)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserGuid == guid);
        if (user is null)
        {
            return Ok(new ApiErrorDto
            {
                Message = "User does not exist.",
                Attempted = guid.ToString(),
                Timestamp = default,
                Suggestions = "Ensure the guid matches the correct user."
            });
        }
        return Ok(user);
    }
}

public class LoginModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}