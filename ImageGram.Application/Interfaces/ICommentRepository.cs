using ImageGram.Domain;

namespace ImageGram.Application.Interfaces;

/// <summary>
/// Interface for CommentRepository
/// </summary>
public interface ICommentRepository
{
    Task CreateCommentAsync(Comment comment);
    Task DeleteCommentAsync(string commentId, string postId);
    Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(string postId);
}
