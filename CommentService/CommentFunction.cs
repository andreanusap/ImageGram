using ImageGram.Application.RequestModels;
using ImageGram.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommentService;

public class CommentFunction
{
    private readonly ICommentFunctionService _commentFunctionService;

    public CommentFunction(ICommentFunctionService commentFunctionService)
    {
        _commentFunctionService = commentFunctionService;
    }

    [FunctionName("Comment")]
    public async Task<IActionResult> RunSave(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        try
        {
            if (req.Body is null)
            {
                return new BadRequestObjectResult("Invalid request.");
            }

            var content = await new StreamReader(req.Body).ReadToEndAsync();
            var commentRequestModel = JsonConvert.DeserializeObject<CommentRequestModel>(content);

            var post = await _commentFunctionService.SaveCommentAsync(commentRequestModel);

            return new OkObjectResult(post);

        }
        catch (Exception ex)
        {
            log.LogError(ex.Message);
            log.LogError(ex.StackTrace);
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [FunctionName("DeleteComment")]
    public async Task<IActionResult> RunDelete([HttpTrigger(AuthorizationLevel.Function, "delete", 
        Route = "deletecomment")] HttpRequestMessage req,
        ILogger log)
    {
        try
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.RequestUri.Query);
            if (query is null)
            {
                return new BadRequestResult();
            }
            await _commentFunctionService.DeleteCommentAsync(query.Get("commentId"), query.Get("postId"));
            return new OkObjectResult("Comment deleted successfully.");
        }
        catch (Exception ex)
        {
            log.LogError(ex.Message);
            log.LogError(ex.StackTrace);
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}
