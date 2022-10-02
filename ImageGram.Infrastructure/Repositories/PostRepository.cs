using ImageGram.Application.Interfaces;
using ImageGram.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace ImageGram.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Database _database;
        private readonly Container _container;

        public PostRepository(CosmosClient cosmosClient, IConfiguration configuration)
        {
            _cosmosClient = cosmosClient;
            _database = _cosmosClient.GetDatabase(configuration["DatabaseName"]);
            _container = _database.GetContainer(configuration["CollectionName"]);
        }

        /// <summary>
        /// Create and save new Post
        /// </summary>
        /// <param name="post">The Post model</param>
        /// <returns></returns>
        public async Task CreatePostAsync(Post post)
        {
            await _container.CreateItemAsync(post, new PartitionKey(post.UserId));
        }
    }
}
