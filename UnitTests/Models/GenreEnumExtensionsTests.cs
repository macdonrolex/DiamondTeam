using ContosoCrafts.WebSite.Models;
using NUnit.Framework;

namespace UnitTests.Models;

/// <summary>
///  Unit tests for GenreEnumExtensionsTests.cs 
/// </summary>
[TestFixture]
public class GenreEnumExtensionsTests
{
    #region DisplayGenre

    /// <summary>
    /// Test whether DisplayGenre returns the display text of every genre.
    /// </summary>
    [Test]
    public void DisplayGenre_Valid_Should_Display_Genre_Game()
    {
        // Arrange
        var productModelAction = new ProductModel();
        var productModelAdventure = new ProductModel();
        var productModelPlatform = new ProductModel();
        var productModelShooter = new ProductModel();
        var productModelFighting = new ProductModel();
        var productModelStealth = new ProductModel();
        var productModelStrategy = new ProductModel();
        var productModelSandbox = new ProductModel();
        var productModelRhythm = new ProductModel();
        var productModelUnknown = new ProductModel();
        var productModelUndefined = new ProductModel();

        // Set up Action genre
        productModelAction.Genre = GenreEnum.Action;
        productModelAdventure.Genre = GenreEnum.Adventure;
        productModelPlatform.Genre = GenreEnum.Platform;
        productModelShooter.Genre = GenreEnum.Shooter;
        productModelFighting.Genre = GenreEnum.Fighting;
        productModelStealth.Genre = GenreEnum.Stealth;
        productModelStrategy.Genre = GenreEnum.Strategy;
        productModelSandbox.Genre = GenreEnum.Sandbox;
        productModelRhythm.Genre = GenreEnum.Rhythm;
        productModelUnknown.Genre = GenreEnum.Unknown;
        productModelUndefined.Genre = GenreEnum.Undefined;

        // Act
        var resultAction = productModelAction.Genre.DisplayGenre();
        var resultAdventure = productModelAdventure.Genre.DisplayGenre();
        var resultPlatform = productModelPlatform.Genre.DisplayGenre();
        var resultShooter = productModelShooter.Genre.DisplayGenre();
        var resultFighting = productModelFighting.Genre.DisplayGenre();
        var resultStealth = productModelStealth.Genre.DisplayGenre();
        var resultStrategy = productModelStrategy.Genre.DisplayGenre();
        var resultSandbox = productModelSandbox.Genre.DisplayGenre();
        var resultRhythm = productModelRhythm.Genre.DisplayGenre();
        var resultUnknown = productModelUnknown.Genre.DisplayGenre();
        var resultUndefined = productModelUndefined.Genre.DisplayGenre();
        
        // Reset
        
        // Not needed

        // Assert

        // Check the returned strings
        Assert.That(resultAction, Is.EqualTo("Action Game"));
        Assert.That(resultAdventure, Is.EqualTo("Adventure Game"));
        Assert.That(resultPlatform, Is.EqualTo("Platforming Game"));
        Assert.That(resultShooter, Is.EqualTo("Shooter Game"));
        Assert.That(resultFighting, Is.EqualTo("Fighting Game"));
        Assert.That(resultStealth, Is.EqualTo("Stealth Game"));
        Assert.That(resultStrategy, Is.EqualTo("Strategy Game"));
        Assert.That(resultSandbox, Is.EqualTo("Sandbox Game"));
        Assert.That(resultRhythm, Is.EqualTo("Rhythm Game"));
        Assert.That(resultUnknown, Is.EqualTo("Unknown Game Genre"));
        Assert.That(resultUndefined, Is.EqualTo(""));
    }

    #endregion DisplayGenre
}