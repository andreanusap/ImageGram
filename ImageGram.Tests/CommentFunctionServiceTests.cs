using ImageGram.Application.Interfaces;
using ImageGram.Application.RequestModels;
using ImageGram.Application.Services;
using ImageGram.Domain;

namespace ImageGram.Tests;

public class CommentFunctionServiceTests
{
    static readonly Fixture Fixture = new Fixture();

    private ICommentFunctionService _commentFunctionService;
    private Mock<ICommentRepository> mockCommentRepository = new Mock<ICommentRepository>();

    [Fact]
    public async void SaveCommentAsync_ReturnsComment()
    {
        var commentRequestModel = Fixture.Create<CommentRequestModel>();

        mockCommentRepository
            .Setup(s => s.CreateCommentAsync(It.IsAny<Comment>()))
            .Returns(Task.CompletedTask);

        _commentFunctionService = new CommentFunctionService(mockCommentRepository.Object);

        var result = await _commentFunctionService.SaveCommentAsync(commentRequestModel);

        mockCommentRepository.Verify(r => r.CreateCommentAsync(It.IsAny<Comment>()), Times.Once);

        result.Should().BeOfType<Comment>();
        result.CommentText.Should().Be(commentRequestModel.commentText);
        result.PostId.Should().Be(commentRequestModel.postId);
        result.UserId.Should().Be(commentRequestModel.userId);
    }

    [Fact]
    public async void DeleteCommentAsync_CallsDeleteCommentAsyncOnce()
    {
        mockCommentRepository
            .Setup(s => s.DeleteCommentAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _commentFunctionService = new CommentFunctionService(mockCommentRepository.Object);

        await _commentFunctionService.DeleteCommentAsync(It.IsAny<string>(), It.IsAny<string>());

        mockCommentRepository.Verify(r => r.DeleteCommentAsync(It.IsAny<string>(), It.IsAny<string>()));
    }
}
