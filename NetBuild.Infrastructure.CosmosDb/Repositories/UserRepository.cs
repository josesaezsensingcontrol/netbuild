using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using NetBuild.Domain.Repositories;
using NetBuild.Infrastructure.CosmosDb.Configuration;
using System.Linq.Expressions;
using User = NetBuild.Domain.Entities.User;

namespace NetBuild.Infrastructure.CosmosDb.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Container _userContainer;

        public UserRepository(CosmosClient client, IOptions<CosmosDbConfiguration> cosmosDbConfig, IOptions<CosmosContainersConfiguration> containersConfig) {
            _userContainer = client.GetContainer(cosmosDbConfig.Value.DatabaseName, containersConfig.Value.UserContainerName);
        }

        public async Task<string> AddAsync(User entity)
        {
            ItemResponse<User> userResponse = await _userContainer.CreateItemAsync(entity, new PartitionKey(entity.Id.ToString()));
            
            return userResponse.Resource.Id;
        }

        public async Task DeleteAsync(string id)
        {
            var value = await GetByIdAsync(id);
            if (value == null)
            {
                return;
            }

            await _userContainer.DeleteItemAsync<User>(id.ToString(), new PartitionKey(id.ToString()));
        }

        public async Task<IEnumerable<User>> GetAsync(Expression<Func<User, bool>>? predicate = null)
        {
            var users = new List<User>();

            FeedIterator<User> query;
            QueryRequestOptions options = new QueryRequestOptions() { MaxBufferedItemCount = 100, MaxConcurrency = 10 };

            if (predicate == null)
            {
                query = _userContainer.GetItemQueryIterator<User>("SELECT * FROM users", requestOptions: options);
            }
            else 
            {
                query = _userContainer.GetItemLinqQueryable<User>(requestOptions: options).Where(predicate).ToFeedIterator();
            }

            using (query) {
                while (query.HasMoreResults)
                {
                    foreach (var user in await query.ReadNextAsync())
                    {
                        users.Add(user);
                    }
                }
            }

            return users;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return (await _userContainer.ReadItemAsync<User>(id.ToString(), new PartitionKey(id.ToString()))).Resource;
        }

        public async Task UpdateAsync(User entity)
        {
            await _userContainer.UpsertItemAsync(entity, new PartitionKey(entity.Id.ToString()), new ItemRequestOptions { EnableContentResponseOnWrite = false });
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using (FeedIterator<User> iterator = _userContainer.GetItemLinqQueryable<User>()
                      .Where(u => u.Email == email)
                      .Take(1)
                      .ToFeedIterator())
            {
                if (iterator.HasMoreResults) { 
                    return (await iterator.ReadNextAsync()).Resource.FirstOrDefault();
                }
            }

            return null;
        }
    }
}