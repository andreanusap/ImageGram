using ImageGram.Application.Interfaces;
using ImageGram.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace ImageGram.Infrastructure.Repositories;

public class NewsFeedRepository : INewsFeedRepository
{
    private readonly CosmosClient _cosmosClient;
    private readonly Database _database;
    private readonly Container _container;

    public NewsFeedRepository(CosmosClient cosmosClient, IConfiguration configuration)
    {
        _cosmosClient = cosmosClient;
        _database = _cosmosClient.GetDatabase(configuration["DatabaseName"]);
        _container = _database.GetContainer(configuration["CollectionName"]);
    }

    /// <summary>
    /// Get all newsfeed
    /// </summary>
    /// <returns>List of Newsfeed</returns>
    public async Task<IEnumerable<NewsFeed>> GetAllNewsFeedsAsync()
    {
        var newsFeeds = new List<NewsFeed>();
        using FeedIterator<NewsFeed> iterator = _container.GetItemQueryIterator<NewsFeed>(
            queryText: "SELECT * FROM newsfeeds"
            );

        while (iterator.HasMoreResults)
        {
            FeedResponse<NewsFeed> response = await iterator.ReadNextAsync();

            // Iterate query results
            foreach (NewsFeed item in response)
            {
                newsFeeds.Add(item);
            }
        }

        return newsFeeds;
    }

    /// <summary>
    /// Get NewsFeed by User Id
    /// </summary>
    /// <param name="userId">The User Id</param>
    /// <returns>List of NewsFeed</returns>
    public async Task<IEnumerable<NewsFeed>> GetNewsFeedsByUserIdAsync(string userId)
    {
        var newsFeeds = new List<NewsFeed>();
        
        var parameterizedQuery = new QueryDefinition(
            query: "SELECT * FROM newsfeeds n WHERE n.UserId = @userId"
        )
            .WithParameter("@userId", userId);

        using FeedIterator<NewsFeed> filteredFeed = _container.GetItemQueryIterator<NewsFeed>(
            queryDefinition: parameterizedQuery
        );

        while (filteredFeed.HasMoreResults)
        {
            FeedResponse<NewsFeed> response = await filteredFeed.ReadNextAsync();

            foreach (NewsFeed item in response)
            {
                newsFeeds.Add(item);
            }
        }

        return newsFeeds;
    }

    /// <summary>
    /// Get NewsFeed by Post Id
    /// </summary>
    /// <param name="postId">The Post Id</param>
    /// <returns>List of NewsFeed</returns>
    public async Task<NewsFeed> GetNewsFeedsByPostIdAsync(string postId)
    {
        return await _container.ReadItemAsync<NewsFeed>(postId, new PartitionKey(postId));
    }

    /// <summary>
    /// Insert NewsFeed
    /// </summary>
    /// <param name="newsFeed">The NewsFeed model</param>
    /// <returns></returns>
    public async Task InsertNewsFeedAsync(NewsFeed newsFeed)
    {
        await _container.CreateItemAsync(newsFeed, new PartitionKey(newsFeed.PostId));
    }

    /// <summary>
    /// Upsert NewsFeed
    /// </summary>
    /// <param name="newsFeed">The NewsFeed model</param>
    /// <returns></returns>
    public async Task UpsertNewsFeedAsync(NewsFeed newsFeed)
    {
        await _container.UpsertItemAsync(newsFeed, new PartitionKey(newsFeed.PostId));
    }
}
