using ImageGram.Application.Interfaces;
using ImageGram.Application.RequestModels;
using ImageGram.Domain;

namespace ImageGram.Application.Services;

public interface ICommentFunctionService
{
    Task<Comment> SaveCommentAsync(CommentRequestModel commentRequestModel);
    Task DeleteCommentAsync(string commentId, string postId);
}
public class CommentFunctionService : ICommentFunctionService
{
    private readonly ICommentRepository _commentRepository;

    public CommentFunctionService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    /// <summary>
    /// Delete Comment by Comment Id and Post Id
    /// </summary>
    /// <param name="commentId">The Comment Id</param>
    /// <param name="postId">The Post Id</param>
    /// <returns></returns>
    public async Task DeleteCommentAsync(string commentId, string postId)
    {
        await _commentRepository.DeleteCommentAsync(commentId, postId);
    }

    /// <summary>
    /// Save new Comment
    /// </summary>
    /// <param name="commentRequestModel">The Coment Request Model</param>
    /// <returns>New Comment</returns>
    public async Task<Comment> SaveCommentAsync(CommentRequestModel commentRequestModel)
    {
        var comment = new Comment(commentRequestModel.postId, commentRequestModel.userId, commentRequestModel.commentText);

        await _commentRepository.CreateCommentAsync(comment);

        return comment;
    }
}
