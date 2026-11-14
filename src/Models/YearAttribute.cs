using System.ComponentModel.DataAnnotations;

namespace ContosoCrafts.WebSite.Models;

/// <summary>
///  Validates that a year is between a start year and a number of years after the current year.
/// </summary>
public class YearAttribute : ValidationAttribute
{
  
}