using ImageGram.Application.Interfaces;
using ImageGram.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace ImageGram.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly CosmosClient _cosmosClient;
    private readonly Database _database;
    private readonly Container _container;

    public CommentRepository(CosmosClient cosmosClient, IConfiguration configuration)
    {
        _cosmosClient = cosmosClient;
        _database = _cosmosClient.GetDatabase(configuration["DatabaseName"]);
        _container = _database.GetContainer(configuration["CollectionName"]);
    }

    /// <summary>
    /// Create new Comment
    /// </summary>
    /// <param name="comment">The comment model</param>
    /// <returns></returns>
    public async Task CreateCommentAsync(Comment comment)
    {
        await _container.CreateItemAsync(comment, new PartitionKey(comment.PostId));
    }

    /// <summary>
    /// Soft delete a comment by Comment Id and Post Id
    /// </summary>
    /// <param name="commentId">The Comment Id</param>
    /// <param name="postId">The Post Id</param>
    /// <returns></returns>
    public async Task DeleteCommentAsync(string commentId, string postId)
    {
        var readResponse = await _container.ReadItemAsync<Comment>(commentId, new PartitionKey(postId));

        Comment comment = readResponse.Resource;

        comment.IsDeleted = true;

        await _container.UpsertItemAsync(comment);

    }

    /// <summary>
    /// Get Comment by Post Id
    /// </summary>
    /// <param name="postId">The Post Id</param>
    /// <returns>List of comments</returns>
    public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(string postId)
    {
        var comments = new List<Comment>();

        var parameterizedQuery = new QueryDefinition(
            query: "SELECT * FROM comments c WHERE c.PostId = @partitionKey"
        )
            .WithParameter("@partitionKey", postId);

        using FeedIterator<Comment> filteredFeed = _container.GetItemQueryIterator<Comment>(
            queryDefinition: parameterizedQuery
        );

        while (filteredFeed.HasMoreResults)
        {
            FeedResponse<Comment> response = await filteredFeed.ReadNextAsync();

            foreach (Comment item in response)
            {
                comments.Add(item);
            }
        }

        return comments;
    }
}
