using ImageGram.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsFeedService
{
    public class NewsFeedFunction
    {
        private readonly INewsFeedFunctionService _newsFeedFunctionService;

        public NewsFeedFunction(INewsFeedFunctionService newsFeedFunctionService)
        {
            _newsFeedFunctionService = newsFeedFunctionService;
        }

        [FunctionName("NewsFeed")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            try
            {
                DateTime? lastSeenPostDate = null;
                var query = System.Web.HttpUtility.ParseQueryString(req.RequestUri.Query);
                if (query is not null && query.Count > 0)
                {
                    var dateCursor = query.Get("lastSeenDate");
                    lastSeenPostDate = DateTime.ParseExact(dateCursor, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                }

                var result = await _newsFeedFunctionService.GetNewsFeedsAsync(lastSeenPostDate);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
