using ImageGram.Application.Interfaces;
using ImageGram.Application.RequestModels;
using ImageGram.Application.Services;
using ImageGram.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace ImageGram.Tests;

public class PostFunctionServiceTests
{
    static readonly Fixture Fixture = new Fixture();

    private IPostFunctionService _postFunctionService;
    private Mock<IPostRepository> mockPostRepository = new Mock<IPostRepository>();
    private Mock<IImageUploader> mockImageUploader = new Mock<IImageUploader>();

    [Fact]
    public async void SavePostAsync_ReturnsPost()
    {

        //Setup mock file using a memory stream
        var content = "Mock Content";
        var fileName = "mock.png";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        var postRequestModel = new PostRequestModel(Fixture.Create<string>(), Fixture.Create<string>(), file);

        mockPostRepository
            .Setup(s => s.CreatePostAsync(It.IsAny<Post>()))
            .Returns(Task.CompletedTask);

        mockImageUploader
            .Setup(s => s.UploadImage(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _postFunctionService = new PostFunctionService(mockPostRepository.Object, mockImageUploader.Object);

        var result = await _postFunctionService.SavePostAsync(postRequestModel);

        mockPostRepository.Verify(r => r.CreatePostAsync(It.IsAny<Post>()), Times.Once);
        mockImageUploader.Verify(r => r.UploadImage(It.IsAny<IFormFile>(), It.IsAny<string>()));

        result.Should().BeOfType<Post>();
        result.Caption.Should().Be(postRequestModel.caption);
        result.UserId.Should().Be(postRequestModel.userId);
        result.ImageUrl.Should().Contain(".jpg");
    }
}