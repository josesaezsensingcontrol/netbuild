using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using NetBuild.Domain.Entities;
using NetBuild.Domain.Repositories;
using NetBuild.Domain.Types;
using NetBuild.Infrastructure.CosmosDb.Configuration;

namespace NetBuild.Infrastructure.CosmosDb.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly Container _historicDataContainer;
        private readonly Container _predictionDataContainer;

        public DataRepository(CosmosClient client, IOptions<CosmosDbConfiguration> cosmosDbConfig, IOptions<CosmosContainersConfiguration> containersConfig)
        {
            _historicDataContainer = client.GetContainer(cosmosDbConfig.Value.DatabaseName, containersConfig.Value.HistoricDataContainerName);
            _predictionDataContainer = client.GetContainer(cosmosDbConfig.Value.DatabaseName, containersConfig.Value.PredictionDataContainerName);
        }

        public async Task AddHistoricDataAsync(string buildingId, string systemId, string dataId, double value, long date)
        {
            await _historicDataContainer.UpsertItemAsync(new DataInputEntry
            {
                Id = date.ToString(),
                DataId = $"{buildingId}-{systemId}-{dataId}",
                Value = value,
                Date = date
            }, new PartitionKey($"{buildingId}-{systemId}-{dataId}"));
        }

        public async Task AddHistoricBulkDataAsync(string buildingId, string systemId, string dataId, List<DataPoint> data)
        {
            var tasks = new List<Task>(data.Count);

            foreach (var item in data)
            {
                tasks.Add(
                    _historicDataContainer.UpsertItemAsync(new DataInputEntry
                    {
                        Id = item.Timestamp.ToString(),
                        DataId = $"{buildingId}-{systemId}-{dataId}",
                        Value = item.Value,
                        Date = item.Timestamp
                    }, new PartitionKey($"{buildingId}-{systemId}-{dataId}"))
                );
            }

            await Task.WhenAll(tasks);
        }

        public async Task<List<DataInputEntry>> GetHistoricDataAsync(string buildingId, string systemId, string dataId, long fromDate, long toDate)
        {
            var entries = new List<DataInputEntry>();

            QueryRequestOptions options = new QueryRequestOptions() { MaxBufferedItemCount = 100, MaxConcurrency = 10 };

            FeedIterator<DataInputEntry> query = _historicDataContainer.GetItemLinqQueryable<DataInputEntry>(requestOptions: options)
                .Where(x => 
                    x.DataId == $"{buildingId}-{systemId}-{dataId}" 
                    && x.Date >= fromDate 
                    && x.Date <= toDate
                )
                .ToFeedIterator();

            using (query)
            {
                while (query.HasMoreResults)
                {
                    foreach (var entry in await query.ReadNextAsync())
                    {
                        entries.Add(entry);
                    }
                }
            }

            return entries;
        }

        public async Task<bool> DeleteAllHistoricDataAsync(string partitionKey)
        {
            return (await _historicDataContainer.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(partitionKey))).IsSuccessStatusCode;
        }

        public async Task DeleteHistoricDataAsync(string id, string partitionKey)
        {
            await _historicDataContainer.DeleteItemAsync<DataInputEntry>(id, new PartitionKey(partitionKey));
        }

        public async Task AddPredictionDataAsync(string buildingId, string systemId, string dataId, double value, long date)
        {
            await _predictionDataContainer.UpsertItemAsync(new DataInputEntry
            {
                Id = date.ToString(),
                DataId = $"{buildingId}-{systemId}-{dataId}",
                Value = value,
                Date = date
            }, new PartitionKey($"{buildingId}-{systemId}-{dataId}"));
        }

        public async Task AddPredictionBulkDataAsync(string buildingId, string systemId, string dataId, List<DataPoint> predictions)
        {
            var tasks = new List<Task>(predictions.Count);

            foreach (var item in predictions)
            {
                tasks.Add(
                    _predictionDataContainer.UpsertItemAsync(new DataInputEntry
                    {
                        Id = item.Timestamp.ToString(),
                        DataId = $"{buildingId}-{systemId}-{dataId}",
                        Value = item.Value,
                        Date = item.Timestamp
                    }, new PartitionKey($"{buildingId}-{systemId}-{dataId}"))
                );
            }

            await Task.WhenAll(tasks);
        }

        public async Task<List<DataInputEntry>> GetPredictionDataAsync(string buildingId, string systemId, string dataId, long fromDate, long toDate)
        {
            var entries = new List<DataInputEntry>();

            QueryRequestOptions options = new QueryRequestOptions() { MaxBufferedItemCount = 100, MaxConcurrency = 10 };

            FeedIterator<DataInputEntry> query = _predictionDataContainer.GetItemLinqQueryable<DataInputEntry>(requestOptions: options)
                .Where(x => 
                    x.DataId == $"{buildingId}-{systemId}-{dataId}" 
                    && x.Date >= fromDate 
                    && x.Date <= toDate
                )
                .ToFeedIterator();

            using (query)
            {
                while (query.HasMoreResults)
                {
                    foreach (var entry in await query.ReadNextAsync())
                    {
                        entries.Add(entry);
                    }
                }
            }

            return entries;
        }

        public async Task DeletePredictionDataAsync(string id, string partitionKey)
        {
            await _predictionDataContainer.DeleteItemAsync<DataInputEntry>(id, new PartitionKey(partitionKey));
        }

        public async Task<bool> DeleteAllPredictionDataAsync(string partitionKey)
        {
            return (await _predictionDataContainer.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(partitionKey))).IsSuccessStatusCode;
        }
    }
}
