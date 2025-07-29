using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VideoGameFinder.Controllers;

[ApiController]
[Route("api/health")]
public class HealthCheckController: ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthCheckController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet("get-Database-health")]
    public async Task<IActionResult> CheckDb()
    {
        var report = await _healthCheckService.CheckHealthAsync(item => item.Name == "videogamedatabase");

        if (report.Status == HealthStatus.Healthy)
        {
            return Ok("Healthy");
        }

        return Ok("Unhealthy");
    }
}