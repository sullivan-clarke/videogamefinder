namespace VideoGameFinder.Controllers.ControllerDtos;

public class ApiErrorDto
{
    public string Message { get; set; } = "";
    public string Attempted { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Suggestions { get; set; } = "";
}