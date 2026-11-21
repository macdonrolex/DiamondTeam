namespace ContosoCrafts.WebSite.Models;

/// <summary>
/// Comments entered by the user about the Product
/// </summary>
public class CommentModel
{
    // The ID for this comment, use a Guid so it is always unique
    public string Id { get; set; } = System.Guid.NewGuid().ToString();

    // The Comment
    public string Comment { get; set; }
    
    // The number of likes for this comment
    public int LikeCount { get; set; } = 0;
    
    // The number of dislikes for this comment
    public int DislikeCount { get; set; } = 0;
}