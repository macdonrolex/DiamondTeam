using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ContosoCrafts.WebSite.Models
{
    /// <summary>
    ///  Data structure for storing a gaming blog post.Each entry in the JSON database
    ///  represents one post about one game, including its links, image, author(Maker)...etc.
    /// </summary>
    public class ProductModel
    {
        // Unique identifier of the post
        public string Id { get; set; }
        public string Maker { get; set; }

        [JsonPropertyName("img")]
        public string Image { get; set; }
        
        [StringLength (maximumLength: 33, MinimumLength = 1, ErrorMessage = "The Title should have a length of more than {2} and less than {1}")]
        public string Title { get; set; }

        public string Description { get; set; }

        public int[] Ratings { get; set; }
        
		public string ShopLink { get; set; }
		public string RatingLink { get; set; }
		public string Post { get; set; }
        // Store the Comments entered by the users on this product
        public List<CommentModel> CommentList { get; set; } = new List<CommentModel>();

        public override string ToString() => JsonSerializer.Serialize<ProductModel>(this);


    }
}