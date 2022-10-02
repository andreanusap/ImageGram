using Microsoft.AspNetCore.Http;

namespace ImageGram.Application.Validations;

public static class PostRequestValidation
{
    /// <summary>
    /// Validate HttpRequest for Post function
    /// </summary>
    /// <param name="request">The HttpRequest</param>
    /// <returns>List of error message</returns>
    public static IEnumerable<string> ValidatePostRequest(HttpRequest request)
    {
        if (request.Form is null)
        {
            yield return "Request data is invalid.";
        }

        if (string.IsNullOrWhiteSpace(request.Form["userId"]))
        {
            yield return "User Id is empty.";
        }

        var files = request.Form.Files;
        if (files is null && files.Count < 0)
        {
            yield return "Image is empty.";
        }

        if (!IsValidExtension(files[0]))
        {
            yield return "Image data type is not supported.";
        }

        if (!IsValidSize(files[0]))
        {
            yield return "Image exceed the maximum allowed size.";
        }
    }

    /// <summary>
    /// Validate form or image extension
    /// </summary>
    /// <param name="file">The file</param>
    /// <returns>Boolean indicating the success or failure of the validation</returns>
    private static bool IsValidExtension(IFormFile file)
    {
        string[] _extensions = new string[] { ".jpg", ".png", ".bmp" };
        var extension = Path.GetExtension(file.FileName);
        if (!_extensions.Contains(extension.ToLower()))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validate file or image size
    /// </summary>
    /// <param name="file">The file</param>
    /// <returns>Boolean indicating the success or failure of the validation</returns>
    private static bool IsValidSize(IFormFile file)
    {
        var _maxFileSize = 100 * 1024 * 1024;
        if (file.Length > _maxFileSize)
        {
            return false;
        }

        return true;
    }
}
