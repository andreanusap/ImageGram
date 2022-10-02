using Newtonsoft.Json;

namespace ImageGram.Domain;

public class NewsFeed
{
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonProperty("postId")]
    public string PostId { get; set; }
    [JsonProperty("caption")]
    public string Caption { get; set; }
    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
    [JsonProperty("postComments")]
    public IEnumerable<PostComment> PostComments { get; set; }
    [JsonProperty("commentCount")]
    public int CommentCount { get; set; }

    public NewsFeed(string userId, string postId, string caption, string imageUrl, DateTime createdAt, IEnumerable<PostComment> postComments)
    {
        Id = postId;
        UserId = userId;
        PostId = postId;
        Caption = caption;
        ImageUrl = imageUrl;
        CreatedAt = createdAt;
        PostComments = postComments;
    }
}

public class PostComment 
{
    [JsonProperty("commentId")]
    public string CommentId { get; set; }
    [JsonProperty("userId")]
    public string UserId { get; set; }
    [JsonProperty("comment")]
    public string Comment { get; set; }
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
}

