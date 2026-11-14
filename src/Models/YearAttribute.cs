using System;
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
        switch (value)
        {
            // The input value is an int year
            case int year:
                // Value must in range [startYear, thisYear + extraYears]
                if (year < StartYear)
                {
                    return false;
                }

                if (year > EndYear)
                {
                    return false;
                }

                break;
            
            // If the type of given value is null or something else
            default:
                return false;
        }
        
        return true;
    }
}