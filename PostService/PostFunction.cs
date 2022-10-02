using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ImageGram.Application.Extensions;
using ImageGram.Application.RequestModels;
using ImageGram.Application.Services;
using System.Threading.Tasks;
using ImageGram.Application.ResponseModels;
using System;
using System.Net;

namespace PostService;



public class PostFunction
{
    private readonly IPostFunctionService _postFunctionService;

    public PostFunction(IPostFunctionService postFunctionService)
    {
        _postFunctionService = postFunctionService;
    }

    [FunctionName("post")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        try
        {
            var isValidPostRequest = req.ParsePostRequest(out PostRequestModel postRequestModel);

            if (!isValidPostRequest.Item1)
            {
                return new BadRequestObjectResult(isValidPostRequest.Item2);
            }

            var post = await _postFunctionService.SavePostAsync(postRequestModel);

            return new OkObjectResult(post);

        }
        catch (Exception ex)
        {
            log.LogError(ex.Message);
            log.LogError(ex.StackTrace);
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}
