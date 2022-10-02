using Microsoft.AspNetCore.Http;

namespace ImageGram.Application.Interfaces;

/// <summary>
/// Interface for ImageUploader
/// </summary>
public interface IImageUploader
{
    Task UploadImage(IFormFile imageFile, string imageFilename);
    Task ConvertAndUploadImage(Stream imageStream, string imageFilename);
}
