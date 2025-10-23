using NetBuild.App.Core.ApiModel;
using NetBuild.Domain.Types;

namespace NetBuild.App.Core.Services.Interface
{
    public interface IDataService
    {
        Task AddHistoricDataAsync(string buildingId, string systemId, string dataId, double value, long timestamp);
        Task AddHistoricBulkDataAsync(string buildingId, string systemId, string dataId, List<DataPoint> data);
        Task<IEnumerable<DataInputEntry>> GetHistoricDataAsync(string buildingId, string systemId, string dataId, long fromDate, long toDate);
        Task DeleteHistoricDataEntryAsync(string id, string partitionKey);
        Task<bool> DeleteAllHistoricDataAsync(string partitionKey);
        Task AddPredictionDataAsync(string buildingId, string systemId, string dataId, double value, long timestamp);
        Task AddPredictionBulkDataAsync(string buildingId, string systemId, string dataId, List<DataPoint> predictions);
        Task<IEnumerable<DataInputEntry>> GetPredictionDataAsync(string buildingId, string systemId, string dataId, long fromDate, long toDate);
        Task DeletePredictionDataEntryAsync(string id, string partitionKey);
        Task<bool> DeleteAllPredictionDataAsync(string partitionKey);
    }
}
