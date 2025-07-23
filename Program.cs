using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VideoGameFinder.Model;
using VideoGameFinder.DbSeed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//For connecting a real, external db, this might be where you connect it 
builder.Services.AddDbContext<VideoGameContext>();

// Add services to the container.

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddNpgSql(
    "host=db;port=5432;database=videogamedatabase;username=sullivan;password=password", name: "videogamedatabase");

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Video Game Finder", 
        Description = "A program to find the best selling Playstation 4 games.", 
        Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI( c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json/", "Video Game Finder API V1");
});

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapControllers();
app.MapHealthChecks("/health/db");

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<VideoGameContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

try
{
    await db.Database.MigrateAsync();
    await DbInitializer.SeedDbAsync(db); //Seeds database
}
catch(Exception ex)
{
    logger.LogError(ex, "An error occured during migration");
    return;
}

app.Run();
