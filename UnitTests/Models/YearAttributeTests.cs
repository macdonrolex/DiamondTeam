using ContosoCrafts.WebSite.Models;
using NUnit.Framework;

namespace UnitTests.Models;

/// <summary>
///  Unit tests for YearAttribute.cs 
/// </summary>
[TestFixture]
public class YearAttributeTests
{
    #region IsValid

    /// <summary>
    /// Test whether the attribute can validate the range we want when given year is smaller
    /// than the start year.
    /// </summary>
    [Test]
    public void IsValid_InValid_Smaller_Than_StartYear_Should_Return_False()
    {
        // Arrange
        
        // Set the year attribute with start year = 100, and extra year = 5
        var data = new YearAttribute(100, 5);
        
        // Act
        var result = data.IsValid(99);

        // Reset

        // Not needed

        // Assert
        Assert.That(result, Is.EqualTo(false));
    }

    #endregion IsValid
}