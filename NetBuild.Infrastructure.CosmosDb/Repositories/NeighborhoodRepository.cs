using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using NetBuild.Domain.Entities;
using NetBuild.Domain.Repositories;
using NetBuild.Infrastructure.CosmosDb.Configuration;
using System.Linq.Expressions;

namespace NetBuild.Infrastructure.CosmosDb.Repositories
{
    public class NeighborhoodRepository : INeighborhoodRepository
    {
        private readonly Container _neighborhoodContainer;

        public NeighborhoodRepository(CosmosClient client, IOptions<CosmosDbConfiguration> cosmosDbConfig, IOptions<CosmosContainersConfiguration> containersConfig)
        {
            _neighborhoodContainer = client.GetContainer(cosmosDbConfig.Value.DatabaseName, containersConfig.Value.NeighborhoodContainerName);
        }

        public async Task<Neighborhood> AddAsync(Neighborhood entity)
        {
            return (await _neighborhoodContainer.CreateItemAsync(entity, new PartitionKey(entity.Id))).Resource;
        }

        public async Task DeleteAsync(string id)
        {
            await _neighborhoodContainer.DeleteItemAsync<Neighborhood>(id, new PartitionKey(id));
        }

        public async Task<IEnumerable<Neighborhood>> GetAsync(Expression<Func<Neighborhood, bool>>? predicate = null)
        {
            var neighborhoods = new List<Neighborhood>();

            FeedIterator<Neighborhood> query;
            QueryRequestOptions options = new QueryRequestOptions() { MaxBufferedItemCount = 100, MaxConcurrency = 10 };

            if (predicate == null)
            {
                query = _neighborhoodContainer.GetItemQueryIterator<Neighborhood>("SELECT * FROM neighborhoods", requestOptions: options);
            }
            else
            {
                query = _neighborhoodContainer.GetItemLinqQueryable<Neighborhood>(requestOptions: options).Where(predicate).ToFeedIterator();
            }

            using (query)
            {
                while (query.HasMoreResults)
                {
                    foreach (var neighborhood in await query.ReadNextAsync())
                    {
                        neighborhoods.Add(neighborhood);
                    }
                }
            }

            return neighborhoods;
        }

        public async Task<Neighborhood?> GetByIdAsync(string id)
        {
            return (await _neighborhoodContainer.ReadItemAsync<Neighborhood>(id, new PartitionKey(id))).Resource;
        }

        public async Task UpdateAsync(Neighborhood entity)
        {
            await _neighborhoodContainer.UpsertItemAsync<Neighborhood>(entity, new PartitionKey(entity.Id));
        }

        public async Task PatchAsync(string id, Dictionary<string, object> fieldsToUpdate)
        {
            await _neighborhoodContainer.PatchItemAsync<Neighborhood>(
                id,
                new PartitionKey(id),
                fieldsToUpdate.Select(x => PatchOperation.Add($"/{x.Key}", x.Value)).ToList(),
                new() { EnableContentResponseOnWrite = false }
            );
        }
    }
}
