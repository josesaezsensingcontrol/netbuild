using NetBuild.Domain.Entities;
using NetBuild.Domain.Types;

namespace NetBuild.Domain.Repositories
{
    public interface IDataRepository
    {
        Task AddHistoricDataAsync(string buildingId, string systemId, string dataId, double value, long date);
        Task AddHistoricBulkDataAsync(string buildingId, string systemId, string dataId, List<DataPoint> data);
        Task<List<DataInputEntry>> GetHistoricDataAsync(string buildingId, string systemId, string dataId, long fromDate, long toDate);
        Task DeleteHistoricDataAsync(string id, string partitionKey);
        Task<bool> DeleteAllHistoricDataAsync(string partitionKey);
        Task AddPredictionDataAsync(string buildingId, string systemId, string dataId, double value, long date);
        Task AddPredictionBulkDataAsync(string buildingId, string systemId, string dataId, List<DataPoint> predictions);
        Task<List<DataInputEntry>> GetPredictionDataAsync(string buildingId, string systemId, string dataId, long fromDate, long toDate);
        Task DeletePredictionDataAsync(string id, string partitionKey);
        Task<bool> DeleteAllPredictionDataAsync(string partitionKey);
    }
}
