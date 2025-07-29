using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VideoGameFinder.Controllers.ControllerDtos;
using VideoGameFinder.Model;

namespace VideoGameFinder.ApiRelated;

public abstract class EndpointMethods
{
    public static string CopiesSoldtoInt(List<VideoGame> games)
    {
        var copiesSoldInt = new List<double>();
        var copiesSoldString = games.Select(x => x.CopiesSold).ToList();

        foreach (var copiesString in copiesSoldString)
        {
            int multiplier;
            var parts = copiesString.Split(' ');

            if (parts[1] == "million")
            {
                multiplier = 1000000;
            } else if (parts[1] == "billion")
            {
                multiplier = 1000000000;
            }
            else
            {
                multiplier = 0;
            }
            
            var copiesInt = double.Parse(parts[0]) * multiplier;
            copiesSoldInt.Add(copiesInt);
        }
        var maxCopiesSold = copiesSoldInt.Max();
        var maxCopiesSoldIndex = copiesSoldInt.IndexOf(maxCopiesSold);
        var maxCopiesSoldString = copiesSoldString.ElementAt(maxCopiesSoldIndex);
        
        return maxCopiesSoldString;
    }
    public static string GenerateNewJwtToken(IConfiguration config, User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static bool LoginCheck(DatabaseContext dbContext, string username, string password)
    {
        //Checks if user exists first 
        if (!dbContext.Users.Any(x => x.Username == username))
        {
            return false;
        }
        
        //Checks if password matches user password
        var user = dbContext.Users.FirstOrDefault(x => x.Username == username);

        if (user?.Password != password)
        {
            return false;
        }
        return true; 
    }
}

