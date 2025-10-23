using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using NetBuild.Domain.Repositories;
using NetBuild.Infrastructure.CosmosDb.Configuration;
using System.Linq.Expressions;

namespace NetBuild.Infrastructure.CosmosDb.Repositories
{
    public class SystemRepository : ISystemRepository
    {
        private readonly Container _systemContainer;

        public SystemRepository(CosmosClient client, IOptions<CosmosDbConfiguration> cosmosDbConfig, IOptions<CosmosContainersConfiguration> containersConfig)
        {
            _systemContainer = client.GetContainer(cosmosDbConfig.Value.DatabaseName, containersConfig.Value.SystemContainerName);
        }

        public async Task<Domain.Entities.System> AddAsync(Domain.Entities.System entity)
        {
            return (await _systemContainer.CreateItemAsync(entity, new PartitionKey(entity.BuildingId))).Resource;
        }

        public async Task DeleteAsync(string id, string buildingId)
        {
            await _systemContainer.DeleteItemAsync<Domain.Entities.System>(id, new PartitionKey(buildingId));
        }

        public async Task<IEnumerable<Domain.Entities.System>> GetAsync(Expression<Func<Domain.Entities.System, bool>>? predicate = null)
        {
            var systems = new List<Domain.Entities.System>();

            FeedIterator<Domain.Entities.System> query;
            QueryRequestOptions options = new QueryRequestOptions() { MaxBufferedItemCount = 100, MaxConcurrency = 10 };

            if (predicate == null)
            {
                query = _systemContainer.GetItemQueryIterator<Domain.Entities.System>("SELECT * FROM systems", requestOptions: options);
            }
            else
            {
                query = _systemContainer.GetItemLinqQueryable<Domain.Entities.System>(requestOptions: options).Where(predicate).ToFeedIterator();
            }

            using (query)
            {
                while (query.HasMoreResults)
                {
                    foreach (var system in await query.ReadNextAsync())
                    {
                        systems.Add(system);
                    }
                }
            }

            return systems;
        }

        public async Task<Domain.Entities.System?> GetByIdAsync(string id, string buildingId)
        {
            return (await _systemContainer.ReadItemAsync<Domain.Entities.System>(id.ToString(), new PartitionKey(buildingId))).Resource;
        }

        public async Task UpdateAsync(Domain.Entities.System entity)
        {
            await _systemContainer.UpsertItemAsync(entity, new PartitionKey(entity.BuildingId));
        }
    }
}
