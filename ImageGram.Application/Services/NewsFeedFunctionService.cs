using ImageGram.Application.Interfaces;
using ImageGram.Domain;

namespace ImageGram.Application.Services;

public interface INewsFeedFunctionService
{
    Task InsertNewPostAsync(Post post);
    Task UpdatePostCommentAsync(Comment comment);
    Task<IEnumerable<NewsFeed>> GetNewsFeedsAsync(DateTime? lastSeenPostDate);
}

public class NewsFeedFunctionService : INewsFeedFunctionService
{
    private readonly INewsFeedRepository _newsFeedRepository;

    public NewsFeedFunctionService(INewsFeedRepository newsFeedRepository)
    {
        _newsFeedRepository = newsFeedRepository;
    }

    /// <summary>
    /// Insert new Post in the NewsFeed
    /// </summary>
    /// <param name="post">The Post model</param>
    /// <returns></returns>
    public async Task InsertNewPostAsync(Post post)
    {
        var newsfeed = new NewsFeed(post.UserId, post.Id, post.Caption, post.ImageUrl, post.CreatedAt, new List<PostComment>());
        await _newsFeedRepository.InsertNewsFeedAsync(newsfeed);
    }

    /// <summary>
    /// Update the Comments of a Post in the NewsFeed
    /// </summary>
    /// <param name="comment">The Coment model</param>
    /// <returns></returns>
    public async Task UpdatePostCommentAsync(Comment comment)
    {
        var newsfeed = await _newsFeedRepository.GetNewsFeedsByPostIdAsync(comment.PostId);

        if (newsfeed is not null)
        {
            var postComments = newsfeed.PostComments.ToList();

            if (comment.IsDeleted == true)
            {
                postComments = postComments
                    .Where(c => c.CommentId != comment.Id)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList();
            }
            else
            {
                var postComment = new PostComment
                {
                    CommentId = comment.Id,
                    Comment = comment.CommentText,
                    CreatedAt = comment.CreatedAt,
                    UserId = comment.UserId
                };

                postComments.Add(postComment);
                postComments = postComments
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList();
            }

            newsfeed.PostComments = postComments;
            newsfeed.CommentCount = postComments.Count;
            await _newsFeedRepository.UpsertNewsFeedAsync(newsfeed);
        }
    }

    /// <summary>
    /// Get NewsFeed filtered by the last seen post date
    /// </summary>
    /// <param name="lastSeenPostDate">The last seen post date</param>
    /// <returns>List of NewsFeed</returns>
    public async Task<IEnumerable<NewsFeed>> GetNewsFeedsAsync(DateTime? lastSeenPostDate = null)
    {
        var newsFeeds = await _newsFeedRepository.GetAllNewsFeedsAsync();

        if (lastSeenPostDate.HasValue)
        {
            newsFeeds = newsFeeds
                .Where(n => n.CreatedAt >= lastSeenPostDate);
        }

        newsFeeds = newsFeeds
            .OrderByDescending(n => n.CommentCount)
            .ToList();

        foreach (var newsFeed in newsFeeds)
        {
            newsFeed.PostComments = newsFeed.PostComments
                .OrderByDescending(c => c.CreatedAt)
                .Take(2)
                .ToList();
        }

        return newsFeeds;
    }
}
