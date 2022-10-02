using ImageGram.Application.Interfaces;
using ImageGram.Application.Services;
using ImageGram.Domain;

namespace ImageGram.Tests;

public class NewsFeedFunctionServiceTests
{
    static readonly Fixture Fixture = new Fixture();

    private INewsFeedFunctionService _newsFeedFunctionService;
    private Mock<INewsFeedRepository> mockNewsFeedRepository = new Mock<INewsFeedRepository>();

    [Fact]
    public async void GetNewsFeedAsync_WithLastSeenPostDate_ShouldReturnNewsFeed()
    {
        var lastDate = DateTime.UtcNow.AddDays(-1);

        var newsFeeds = Fixture.Build<NewsFeed>()
            .With(n => n.PostComments, Fixture.CreateMany<PostComment>().ToList())
            .With(n => n.CreatedAt, DateTime.UtcNow)
            .CreateMany();


        mockNewsFeedRepository
            .Setup(s => s.GetAllNewsFeedsAsync())
            .ReturnsAsync(newsFeeds);

        _newsFeedFunctionService = new NewsFeedFunctionService(mockNewsFeedRepository.Object);

        var result = await _newsFeedFunctionService.GetNewsFeedsAsync(lastDate);

        result.Count().Should().BeLessThanOrEqualTo(result.Count());
        foreach (var newsfeed in result)
        {
            newsfeed.PostComments.Count().Should().BeLessThanOrEqualTo(2);
        }
        result.First().CommentCount.Should().Be(result.Max(r => r.CommentCount));
    }

    [Fact]
    public async void InsertNewPostAsync_CallsInsertNewsFeedAsyncOnce()
    {
        var post = Fixture.Create<Post>();
        var newsFeed = Fixture.Build<NewsFeed>()
            .With(n => n.Caption, post.Caption)
            .With(n => n.PostId, post.Id)
            .With(n => n.UserId, post.UserId)
            .Create();

        mockNewsFeedRepository
            .Setup(s => s.InsertNewsFeedAsync(It.IsAny<NewsFeed>()))
            .Returns(Task.CompletedTask);

        _newsFeedFunctionService = new NewsFeedFunctionService(mockNewsFeedRepository.Object);

        await _newsFeedFunctionService.InsertNewPostAsync(post);

        mockNewsFeedRepository.Verify(r => r.InsertNewsFeedAsync(It.IsAny<NewsFeed>()), Times.Once);
    }

    [Fact]
    public async void UpdatePostCommentAsync_DeletedComment_CallsUpsertNewsFeedAsyncOne()
    {
        var comment = Fixture.Build<Comment>()
            .With(c => c.IsDeleted, true)
            .Create();
        var postComment = Fixture.Build<PostComment>()
            .With(p => p.CommentId, comment.Id)
            .Create();
        var newsfeed = Fixture.Build<NewsFeed>()
            .With(n => n.PostId, comment.PostId)
            .Create();

        mockNewsFeedRepository
            .Setup(s => s.GetNewsFeedsByPostIdAsync(comment.PostId))
            .ReturnsAsync(newsfeed);

        mockNewsFeedRepository
            .Setup(s => s.UpsertNewsFeedAsync(It.IsAny<NewsFeed>()))
            .Returns(Task.CompletedTask);

        _newsFeedFunctionService = new NewsFeedFunctionService(mockNewsFeedRepository.Object);

        await _newsFeedFunctionService.UpdatePostCommentAsync(comment);

        mockNewsFeedRepository.Verify(r => r.UpsertNewsFeedAsync(It.IsAny<NewsFeed>()), Times.Once);
    }

    [Fact]
    public async void UpdatePostCommentAsync_NewComment_CallsUpsertNewsFeedAsyncOne()
    {
        var comment = Fixture.Build<Comment>()
            .With(c => c.IsDeleted, false)
            .Create();
        var postComment = Fixture.Build<PostComment>()
            .With(p => p.CommentId, comment.Id)
            .Create();
        var newsfeed = Fixture.Build<NewsFeed>()
            .With(n => n.PostId, comment.PostId)
            .Create();

        mockNewsFeedRepository
            .Setup(s => s.GetNewsFeedsByPostIdAsync(comment.PostId))
            .ReturnsAsync(newsfeed);

        mockNewsFeedRepository
            .Setup(s => s.UpsertNewsFeedAsync(It.IsAny<NewsFeed>()))
            .Returns(Task.CompletedTask);

        _newsFeedFunctionService = new NewsFeedFunctionService(mockNewsFeedRepository.Object);

        await _newsFeedFunctionService.UpdatePostCommentAsync(comment);

        mockNewsFeedRepository.Verify(r => r.UpsertNewsFeedAsync(It.IsAny<NewsFeed>()), Times.Once);
    }
}