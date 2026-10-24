using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoCrafts.WebSite.Models;

/// <summary>
///     Data structure for storing a gaming blog post.Each entry in the JSON database
///     represents one post about one game, including its links, image, author(Maker)...etc.
/// </summary>
public class ProductModel
{
    // Unique identifier of the post
    public string Id { get; set; }

    // Author of the post
    public string Maker { get; set; }

    // URL of the cover image for the game post (stored as "img" in the JSON file)
    [JsonPropertyName("img")] public string Image { get; set; }

    // Title of the post, must be between 1 and 33 characters
    [StringLength(33, MinimumLength = 1,
        ErrorMessage = "The Title should have a length of more than {2} and less than {1}")]
    public string Title { get; set; }

    // Short description (summary) of the post
    public string Description { get; set; }

    // Ratings given to the post by users(for now, haven't implemented the feature yet)
    public int[] Ratings { get; set; }

    // Link to a store page where the game can be bought (Amazon or others)
    public string ShopLink { get; set; }

    // Link to the Metacritic page of the game in the post
    public string RatingLink { get; set; }

    // Main text content of the post
    public string Post { get; set; }

    // Store the Comments entered by the users on this product
    public List<CommentModel> CommentList { get; set; } = new();

    public override string ToString() => JsonSerializer.Serialize<ProductModel>(this);
}