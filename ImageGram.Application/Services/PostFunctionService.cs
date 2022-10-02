using ImageGram.Application.Interfaces;
using ImageGram.Application.RequestModels;
using ImageGram.Domain;

namespace ImageGram.Application.Services;

public interface IPostFunctionService
{
    Task<Post> SavePostAsync(PostRequestModel postRequestModel);
}

public class PostFunctionService : IPostFunctionService
{
    private readonly IPostRepository _postRepository;
    private readonly IImageUploader _imageUploader;

    public PostFunctionService(IPostRepository postRepository, IImageUploader imageUploader)
    {
        _postRepository = postRepository;
        _imageUploader = imageUploader;
    }

    /// <summary>
    /// Save a new Post
    /// </summary>
    /// <param name="postRequestModel">The Post request model</param>
    /// <returns>New Post</returns>
    public async Task<Post> SavePostAsync(PostRequestModel postRequestModel)
    {
        var newImageName = "IMG_" + Guid.NewGuid().ToString() + "_" +DateTime.UtcNow.Millisecond;
        
        var imageFile = postRequestModel.imageFile;
        var newImageUrl = $"{newImageName}.jpg";

        var post = new Post(postRequestModel.userId, postRequestModel.caption, "/" + newImageUrl);

        await _postRepository.CreatePostAsync(post);

        await _imageUploader.UploadImage(imageFile, $"{newImageName}{Path.GetExtension(imageFile.FileName)}");

        return post;
    }
}
