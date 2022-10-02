using ImageGram.Application.Services;
using ImageGram.Domain;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsFeedAggregatorService
{
    public class NewsFeedAggregatorFunction
    {
        private readonly INewsFeedFunctionService _newsFeedFunctionService;

        public NewsFeedAggregatorFunction(INewsFeedFunctionService newsFeedFunctionService)
        {
            _newsFeedFunctionService = newsFeedFunctionService;
        }

        [FunctionName("PostAggregatorFunction")]
        public async Task RunPostAggregatorFunction([CosmosDBTrigger(
            databaseName: "ImageGramDB",
            collectionName: "Posts",
            ConnectionStringSetting = "ImageGramDBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
            ILogger log)
        {
            try
            {
                if (input != null && input.Count > 0)
                {
                    foreach (var document in input)
                    {
                        var post = JsonConvert.DeserializeObject<Post>(document.ToString());
                        await _newsFeedFunctionService.InsertNewPostAsync(post);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                throw;
            }
            
        }

        [FunctionName("CommentAggregatorFunction")]
        public async Task RunCommentAggregatorFunction([CosmosDBTrigger(
            databaseName: "ImageGramDB",
            collectionName: "Comments",
            ConnectionStringSetting = "ImageGramDBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
            ILogger log)
        {
            try
            {
                if (input != null && input.Count > 0)
                {
                    foreach (var document in input)
                    {
                        var comment = JsonConvert.DeserializeObject<Comment>(document.ToString());
                        await _newsFeedFunctionService.UpdatePostCommentAsync(comment);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                throw;
            }
        }
    }
}
