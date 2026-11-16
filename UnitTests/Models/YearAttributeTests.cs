using System;
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
    
    /// <summary>
    /// Test whether the attribute can validate the range we want when given year is greater
    /// than this year + extra years
    /// </summary>
    [Test]
    public void IsValid_InValid_Greater_Than_ThisYear_Plus_ExtraYears_Should_Return_False()
    {
        // Arrange
        
        // Set the year attribute with start year = 100, and extra years = 5
        var data = new YearAttribute(100, 5);
        
        // Create an year that is greater than this year + extra years
        var dataYear = DateTime.Now.Year + 6;
        
        // Act
        var result = data.IsValid(dataYear);

        // Reset

        // Not needed

        // Assert
        Assert.That(result, Is.EqualTo(false));
    }
    
    /// <summary>
    /// Test whether the attribute can validate the result when the given value is
    /// an invalid data type such as null non-integer.
    /// </summary>
    [Test]
    public void IsValid_InValid_Not_Int_Should_Return_False()
    {
        // Arrange
        
        // Set the year attribute with start year = 100, and extra years = 5
        var data = new YearAttribute(100, 5);
        
        // Create non-int value
        var dataString = "This is a string";
        
        // Act
        var resultNonInt = data.IsValid(dataString);
        var resultNull = data.IsValid(null);

        // Reset

        // Not needed

        // Assert
        Assert.That(resultNonInt, Is.EqualTo(false));
        Assert.That(resultNull, Is.EqualTo(false));
    }
    
    /// <summary>
    /// Test whether the attribute can validate the range we want when
    /// the given year is within the valid range
    /// </summary>
    [Test]
    public void IsValid_Valid_Within_Range_Should_Return_True()
    {
        // Arrange
        
        // Set the year attribute with start year = 100, and extra years = 5
        var data = new YearAttribute(100, 5);
        
        // Create some year values that are within the range
        
        // Normal value
        var dataNormalYear = 2000;
        
        // Valid edge cases
        var dataThisYear = DateTime.Now.Year;
        var dataTYPlusExtra = DateTime.Now.Year + 5;
        var dataStartYear = 100;
        
        // Act
        var resultNormal = data.IsValid(dataNormalYear);
        var resultThisYear = data.IsValid(dataThisYear);
        var resultTYPlusExtra = data.IsValid(dataTYPlusExtra);
        var resultStartYear = data.IsValid(dataStartYear);

        // Reset

        // Not needed

        // Assert
        Assert.That(resultNormal, Is.EqualTo(true));
        Assert.That(resultThisYear, Is.EqualTo(true));
        Assert.That(resultTYPlusExtra, Is.EqualTo(true));
        Assert.That(resultStartYear, Is.EqualTo(true));
    }

    #endregion IsValid
}