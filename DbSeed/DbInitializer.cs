using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using  VideoGameFinder.Model;

namespace VideoGameFinder.DbSeed;


public class DbInitializer
{
    public static async Task SeedDbAsync(DatabaseContext dbcontext) // Might have to add logger as a parameter
    {

        if (!File.Exists("DbSeed/VideoGameData.csv"))
        {
            Console.WriteLine("CSV File not found.");
            return;
        }

        if (dbcontext.VideoGames.Any())
        {
            return;
        }

        //Create parser
        using var reader = new StreamReader("DbSeed/VideoGameData.csv");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null, //Ignores missing headers to do creation of Guid in program
            MissingFieldFound = null
        };
        
        using var csv = new CsvReader(reader, config);

        //Parse through csv
        var gamesToAdd = csv.GetRecords<VideoGame>().ToList();

        //Add changes to db and save them 
        dbcontext.VideoGames.AddRange(gamesToAdd);
        await dbcontext.SaveChangesAsync();

    }
}
    