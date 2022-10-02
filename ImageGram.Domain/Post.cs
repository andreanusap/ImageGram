using Newtonsoft.Json;

namespace ImageGram.Domain;

public class Post
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonProperty("caption")]
    public string Caption { get; set; }

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }

    public Post(string userId, string caption, string imageUrl)
    {
        Id = Guid.NewGuid().ToString() + "_" +DateTime.UtcNow.Millisecond;
        UserId = userId;
        Caption = caption;
        ImageUrl = imageUrl;
        CreatedAt = DateTime.UtcNow;
    }
}
