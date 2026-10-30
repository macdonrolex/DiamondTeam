namespace ContosoCrafts.WebSite.Models;

public enum GenreEnum
{
    Undefined = 0,
    Action = 1,
    Adventure = 2,
    Platform = 3,
    Shooter = 4,
    Fighting = 5,
    Stealth = 6,
    Strategy = 7,
}

public static class GenreEnumExtensions
{
    public static string DisplayGenre(this GenreEnum data)
    {
        return data switch
        {
            GenreEnum.Action => "Action Game",
            GenreEnum.Adventure => "Adventure Game",
            GenreEnum.Platform => "Platforming Game",
            GenreEnum.Shooter => "Shooter Game",
            GenreEnum.Fighting => "Fighting Game",
            GenreEnum.Stealth => "Stealth Game",
            GenreEnum.Strategy => "Strategy Game",
 
            // Default, Unknown
            _ => "",
        };
    }
}