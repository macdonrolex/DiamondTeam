using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoCrafts.WebSite.Attributes;

/// <summary>
///  Validates that a year is between a start year and a number of years after the current year.
/// </summary>
public class YearAttribute : ValidationAttribute
{
    // The earliest year that is accepted
    public readonly int StartYear;

    // How many years after the current year are still accepted
    public readonly int ExtraYears;
    
    // The latest year that is accepted, calculated each time so it never goes out of date
    public int EndYear => DateTime.Now.Year + ExtraYears;
    
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
    
    /// <summary>
    /// Checks whether the value is a year inside the accepted range.
    /// </summary>
    /// <param name="value">The value of the property being validated</param>
    /// <returns>True when the value is a year inside the range, otherwise false</returns>
    public override bool IsValid(object value)
    {

        // Fast fail: the value is null or not a whole-number year
        if ((value is int) == false)
        {
            return false;
        }

        // The value is an int year
        var year = (int)value;

        // Fast fail: earlier than the first accepted year
        if (year < StartYear)
        {
            return false;
        }

        // Fast fail: later than the last accepted year
        if (year > EndYear)
        {
            return false;
        }

        // The year is inside [StartYear, this year + ExtraYears]
        return true;
    }
    
    /// <summary>
    /// Builds the error message with the current limits of the range.
    /// </summary>
    /// <param name="name">The name of the property that failed validation</param>
    /// <returns>The error message shown to the user</returns>
    public override string FormatErrorMessage(string name)
    {
        return $"Choose a year between {StartYear} and {EndYear}.";
    }
}