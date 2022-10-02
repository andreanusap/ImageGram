using ImageGram.Application.RequestModels;
using ImageGram.Application.Validations;
using Microsoft.AspNetCore.Http;

namespace ImageGram.Application.Extensions;

public static class HttpRequestExtensions
{

    /// <summary>
    /// Parse HttpRequest to PostRequestModel
    /// </summary>
    /// <param name="request">The HttpRequest</param>
    /// <param name="postRequestModel">The PostRequestModel as an out parameter</param>
    /// <returns>Tupple of bool indicating the success or failure of parsing process, and list of error message in string.</returns>
    public static (bool, IEnumerable<string>) ParsePostRequest(this HttpRequest request, out PostRequestModel postRequestModel)
    {
        postRequestModel = null;

        var result = PostRequestValidation.ValidatePostRequest(request);

        if (result.Any())
        {
            return (false, result);
        }

        postRequestModel = new PostRequestModel(request.Form["userId"], request.Form["caption"], request.Form.Files[0]);

        return (true, null);
    }
}
