namespace ContosoCrafts.WebSite.Models;

/// <summary>
/// Data sent by the user when it rates a game post.
/// </summary>
public class RatingRequestModel
{
    // The Id of the product
    public string ProductId { get; set; }
    
    // The rating of the product
    public int Rating { get; set; }
}