using ImageGram.Domain;

namespace ImageGram.Application.Interfaces;

/// <summary>
/// Interface for NewsFeedRepository
/// </summary>
public interface INewsFeedRepository
{
    Task InsertNewsFeedAsync(NewsFeed newsFeed);
    Task<IEnumerable<NewsFeed>> GetNewsFeedsByUserIdAsync(string userId);
    Task<IEnumerable<NewsFeed>> GetAllNewsFeedsAsync();
    Task<NewsFeed> GetNewsFeedsByPostIdAsync(string postId);
    Task UpsertNewsFeedAsync(NewsFeed newsFeed);
}
