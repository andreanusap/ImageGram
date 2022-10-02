namespace ImageGram.Application.RequestModels;

public record CommentRequestModel(string postId, string userId, string commentText);