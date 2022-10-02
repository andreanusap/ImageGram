using Azure.Storage.Blobs;
using ImageGram.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ImageGram.Infrastructure;

public class ImageUploader : IImageUploader
{
    private readonly BlobContainerClient _containerClient;

    public ImageUploader(IConfiguration configuration)
    {
        _containerClient = new BlobContainerClient(configuration["StorageConnectionString"], configuration["ContainerName"]);
    }
    
    /// <summary>
    /// Upload Image file
    /// </summary>
    /// <param name="imageFile">The image file</param>
    /// <param name="imageFilename">The image file name to be saved</param>
    /// <returns></returns>
    public async Task UploadImage(IFormFile imageFile, string imageFilename)
    {
        BlobClient client = _containerClient.GetBlobClient(imageFilename);
        using (Stream? data = imageFile.OpenReadStream())
        {
            await client.UploadAsync(data, true, new CancellationToken());
        }
    }

    /// <summary>
    /// Convert and the upload the image
    /// </summary>
    /// <param name="imageStream">The image file stream</param>
    /// <param name="imageFilename">The image file name to be saved</param>
    /// <returns></returns>
    public async Task ConvertAndUploadImage(Stream imageStream, string imageFilename)
    {
        var imageName = imageFilename.Split('.');

        BlobClient client = _containerClient.GetBlobClient(imageName[0]+".jpg");
        
        using (var newStream = new MemoryStream())
        using (Image image = Image.Load(imageStream))
        {
            image.Mutate(x => x.Resize(600, 600));
            image.SaveAsJpeg(newStream, new JpegEncoder());

            newStream.Position = 0;
            await client.UploadAsync(newStream, true, new CancellationToken());
        }
        
    }
}
