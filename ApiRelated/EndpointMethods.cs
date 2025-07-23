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
    
}
