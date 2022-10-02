using ImageGram.Domain;
using System.Threading.Tasks;

namespace ImageGram.Application.Interfaces;

/// <summary>
/// Interface for PostRepository
/// </summary>
public interface IPostRepository
{
    Task CreatePostAsync(Post post);
}
