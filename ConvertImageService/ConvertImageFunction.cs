using ImageGram.Application.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ConvertImageService;

[StorageAccount("StorageConnectionString")]
public class ConvertImageFunction
{
    private readonly ILogger<ConvertImageFunction> _logger;
    private readonly IImageUploader _imageUploader;

    public ConvertImageFunction(ILogger<ConvertImageFunction> logger, IImageUploader imageUploader)
    {
        _logger = logger;
        _imageUploader = imageUploader;
    }

    [FunctionName("ConvertImage")]
    public void Run(
        [BlobTrigger("fileorigin/{name}")] Stream inputBlob,
        string name, ILogger log)
    {
        _logger.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inputBlob.Length} Bytes");

        try
        {
            _imageUploader.ConvertAndUploadImage(inputBlob, name);

            _logger.LogInformation("Image has been successfully converted and saved.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Convert fails. {ex}");
        }
    }
}
