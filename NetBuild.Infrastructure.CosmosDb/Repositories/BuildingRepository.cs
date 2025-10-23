using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using NetBuild.Domain.Entities;
using NetBuild.Domain.Repositories;
using NetBuild.Infrastructure.CosmosDb.Configuration;
using System.Linq.Expressions;

namespace NetBuild.Infrastructure.CosmosDb.Repositories
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly Container _buildingContainer;

        public BuildingRepository(CosmosClient client, IOptions<CosmosDbConfiguration> cosmosDbConfig, IOptions<CosmosContainersConfiguration> containersConfig)
        {
            _buildingContainer = client.GetContainer(cosmosDbConfig.Value.DatabaseName, containersConfig.Value.BuildingContainerName);
        }

        public async Task<Building> AddAsync(Building entity)
        {
            return (await _buildingContainer.CreateItemAsync(entity, new PartitionKey(entity.NeighborhoodId))).Resource;
        }

        public async Task DeleteAsync(string id, string neighborhoodId)
        {
            await _buildingContainer.DeleteItemAsync<Building>(id, new PartitionKey(neighborhoodId));
        }

        public async Task<IEnumerable<Building>> GetAsync(Expression<Func<Building, bool>>? predicate = null)
        {
            var buildings = new List<Building>();

            FeedIterator<Building> query;
            QueryRequestOptions options = new QueryRequestOptions() { MaxBufferedItemCount = 100, MaxConcurrency = 10 };

            if (predicate == null)
            {
                query = _buildingContainer.GetItemQueryIterator<Building>("SELECT * FROM buildings", requestOptions: options);
            }
            else
            {
                query = _buildingContainer.GetItemLinqQueryable<Building>(requestOptions: options).Where(predicate).ToFeedIterator();
            }

            using (query)
            {
                while (query.HasMoreResults)
                {
                    foreach (var building in await query.ReadNextAsync())
                    {
                        buildings.Add(building);
                    }
                }
            }

            return buildings;
        }

        public async Task<Building?> GetByIdAsync(string id, string neighborhoodId)
        {
            return (await _buildingContainer.ReadItemAsync<Building>(id, new PartitionKey(neighborhoodId))).Resource;
        }

        public async Task UpdateAsync(Building entity)
        {
            await _buildingContainer.UpsertItemAsync(entity, new PartitionKey(entity.NeighborhoodId));
        }

        public async Task PatchAsync(string id, string neighborhoodId, Dictionary<string, object> fieldsToUpdate)
        {
            await _buildingContainer.PatchItemAsync<Building>(
                id,
                new PartitionKey(neighborhoodId),
                fieldsToUpdate.Select(x => PatchOperation.Add($"/{x.Key}", x.Value)).ToList(),
                new() { EnableContentResponseOnWrite = false }
            );
        }
    }
}
