using Newtonsoft.Json;

namespace ImageGram.Domain;

public class Comment
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("postId")]
    public string PostId { get; set; }

    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonProperty("comment")]
    public string CommentText { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("deleted")]
    public bool IsDeleted { get; set; }

    public Comment(string postId, string userId, string commentText)
    {
        Id = Guid.NewGuid().ToString() + "_" + DateTime.UtcNow.Millisecond;
        PostId = postId;
        UserId = userId;
        CommentText = commentText;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
}
