using System.ComponentModel.DataAnnotations;

namespace ContosoCrafts.WebSite.Models;

/// <summary>
///  Validates that a year is between a start year and a number of years after the current year.
/// </summary>
public class YearAttribute : ValidationAttribute
{
    // The earliest year that is accepted
    public readonly int StartYear;

    // How many years after the current year are still accepted
    public readonly int ExtraYears;
    
    /// <summary>
    /// Stores the limits of the accepted range.
    /// </summary>
    /// <param name="startYear">The earliest year that is accepted</param>
    /// <param name="extraYears">How many years after the current year are accepted</param>
    public YearAttribute(int startYear, int extraYears)
    {
        StartYear = startYear;
        ExtraYears = extraYears;
    }
}