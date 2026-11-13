using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoCrafts.WebSite.Models;

/// <summary>
///     Data model (structure) for a gaming blog post. Each entry in the JSON database
///     represents one post about one game, including its links, image, author(Maker)...etc.
/// </summary>
public class ProductModel
{
    // Unique identifier of the post
    public string Id { get; set; }

    // The studio or publisher that made the game.
    [Required(ErrorMessage = "Enter the maker or studio name.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Maker must be between 2 and 80 characters.")]
    public string Maker { get; set; }
    
    // The cover image can be a full URL or a path inside this site.
    [Required(ErrorMessage = "Add the cover image link.")]
    [StringLength(2048, ErrorMessage = "The image link must be 2,048 characters or fewer.")]
    [RegularExpression(@"^(https?://\S+|/(?!/)\S+)$", ErrorMessage = "Enter an image URL or a site path like /images/game.jpg.")]
    [JsonPropertyName("img")] 
    public string Image { get; set; }

    // A short, readable name for the game.
    [Required(ErrorMessage = "Enter the game title.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 50 characters.")]
    public string Title { get; set; }

    // A short summary shown with the game.
    [Required(ErrorMessage = "Enter a short game description.")]
    [StringLength(300, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 300 characters.")]
    public string Description { get; set; }

    // Ratings given to the post by users(for now, haven't implemented the feature yet)
    public int[] Ratings { get; set; }

    // Link to a store page where the game can be bought (Amazon or others)
    [StringLength(2048, ErrorMessage = "The shop link must be 2,048 characters or fewer.")]
    [Url(ErrorMessage = "Enter a valid shop link, such as https://example.com.")]
    public string ShopLink { get; set; }

    // Link to the Metacritic page of the game in the post
    [StringLength(2048, ErrorMessage = "The rating link must be 2,048 characters or fewer.")]
    [Url(ErrorMessage = "Enter a valid rating link, such as https://example.com.")]
    public string RatingLink { get; set; }

    // The longer post that explains what players should know.
    [Required(ErrorMessage = "Write the game post.")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "The post must be between 10 and 5,000 characters.")]
    public string Post { get; set; }

    // Store the Comments entered by the users on this product
    public List<CommentModel> CommentList { get; set; } = new();

    // Genre of the game in the post. Default is Undefined
    [Required(ErrorMessage = "Choose a genre.")]
    [Range(1, 10, ErrorMessage = "Choose a genre.")]
    public GenreEnum Genre { get; set; } = GenreEnum.Undefined;
    
    // When this product was first added. Existing records without a date remain null.
    [JsonPropertyName("dateAdded")]
    public DateTime? DateAdded { get; set; }
    
    // The year the game was originally released.
    [Range(-1, 2077, ErrorMessage = "Choose a year between 0 and 2077. -1 for year unknown")]
    [JsonPropertyName("releaseYear")]
    public int ReleaseYear { get; set; }
    
    // Whether the game in the post is multiplayer game.
    public bool IsMultiplayer { get; set; } = false;
    
    /// <summary>
    /// Converts this post to its JSON representation.
    /// </summary>
    /// <returns>A JSON string containing all the properties of this post</returns>
    public override string ToString() => JsonSerializer.Serialize<ProductModel>(this);
}