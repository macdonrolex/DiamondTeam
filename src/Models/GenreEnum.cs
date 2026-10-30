namespace ContosoCrafts.WebSite.Models;

/// <summary>
/// Enum class for representing the genres of the games.
/// </summary>
public enum GenreEnum
{
    // Genre undefined
    Undefined = 0,
    // Action game
    Action = 1,
    // Adventure game
    Adventure = 2,
    // Platforming game
    Platform = 3,
    // Shooter game
    Shooter = 4,
    // Fighting game
    Fighting = 5,
    // Stealth game
    Stealth = 6,
    // Strategy game
    Strategy = 7,
}

/// <summary>
/// Extra features for GenreEnum
/// </summary>
public static class GenreEnumExtensions
{
    /// <summary>
    /// Converts a game genre into the readable text that is shown to users.
    /// </summary>
    /// <param name="data">The game genre to convert</param>
    /// <returns>The display text of the genre, or an empty text when
    /// the genre is Undefined or unknown</returns>
    public static string DisplayGenre(this GenreEnum data)
    {
        // Return the display text
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