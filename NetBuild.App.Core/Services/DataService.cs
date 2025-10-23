using AutoMapper;
using Microsoft.Extensions.Logging;
using NetBuild.App.Core.ApiModel;
using NetBuild.App.Core.Services.Interface;
using NetBuild.Domain.Repositories;
using NetBuild.Domain.Types;

namespace NetBuild.App.Core.Services
{
    public class DataService : IDataService
    {
        private readonly IDataRepository _dataRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DataService> _logger;

        public DataService(IDataRepository dataRepository, IMapper mapper, ILogger<DataService> logger)
        {
            _dataRepository = dataRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddHistoricDataAsync(string buildingId, string systemId, string dataId, double value, long timestamp)
        {
            await _dataRepository.AddHistoricDataAsync(buildingId, systemId, dataId, value, timestamp);
        }

        public async Task AddHistoricBulkDataAsync(string buildingId, string systemId, string dataId, List<DataPoint> data)
        {
            await _dataRepository.AddHistoricBulkDataAsync(buildingId, systemId, dataId, data);
        }

        public async Task<IEnumerable<DataInputEntry>> GetHistoricDataAsync(string buildingId, string systemId, string dataId, long fromDate, long toDate)
        {
            return (await _dataRepository.GetHistoricDataAsync(buildingId, systemId, dataId, fromDate, toDate)).Select(x => _mapper.Map<DataInputEntry>(x));
        }

        public async Task DeleteHistoricDataEntryAsync(string id, string partitionKey)
        {
            await _dataRepository.DeleteHistoricDataAsync(id, partitionKey);
        }

        public async Task<bool> DeleteAllHistoricDataAsync(string partitionKey)
        {
            return await _dataRepository.DeleteAllHistoricDataAsync(partitionKey);
        }

        public async Task AddPredictionDataAsync(string buildingId, string systemId, string dataId, double value, long timestamp)
        {
            await _dataRepository.AddPredictionDataAsync(buildingId, systemId, dataId, value, timestamp);
        }

        public async Task AddPredictionBulkDataAsync(string buildingId, string systemId, string dataId, List<DataPoint> predictions)
        {
            await _dataRepository.AddPredictionBulkDataAsync(buildingId, systemId, dataId, predictions);
        }

        public async Task<IEnumerable<DataInputEntry>> GetPredictionDataAsync(string buildingId, string systemId, string dataId, long fromDate, long toDate)
        {
            return (await _dataRepository.GetPredictionDataAsync(buildingId, systemId, dataId, fromDate, toDate)).Select(x => _mapper.Map<DataInputEntry>(x));
        }

        public async Task DeletePredictionDataEntryAsync(string id, string partitionKey)
        {
            await _dataRepository.DeletePredictionDataAsync(id, partitionKey);
        }

        public async Task<bool> DeleteAllPredictionDataAsync(string partitionKey)
        {
            return await _dataRepository.DeleteAllPredictionDataAsync(partitionKey);
        }
    }
}
