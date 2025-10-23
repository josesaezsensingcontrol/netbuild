using NetBuild.App.Core.ApiModel;
using NetBuild.Domain.Types;

namespace NetBuild.App.Core.Services.Interface
{
    public interface ISystemService
    {
        Task<ApiModel.System?> AddSystemAsync(string systemId, string buildingId, string name, IEnumerable<DataInput> dataInputs, IDictionary<string, string> metadata);
        Task<IEnumerable<ApiModel.System>> GetAllSystemsAsync(string? buildingId = null);
        Task<ApiModel.System?> GetSystemAsync(string systemId, string? buildingId = null);
        Task UpdateSystemAsync(string systemId, string buildingId, string name);
        Task UpdateSystemValuesAsync(string systemId, string buildingId, IDictionary<string, DataPoint> newValues);
        Task UpdateSystemBulkValuesAsync(string systemId, string buildingId, IDictionary<string, List<DataPoint>> newValues);
        Task AddSystemPredictionsAsync(string systemId, string buildingId, IDictionary<string, DataPoint> predictionValues);
        Task AddSystemBulkPredictionsAsync(string systemId, string buildingId, IDictionary<string, List<DataPoint>> predictionValues);
        Task DeleteSystemAsync(string systemId, string buildingId);
    }
}
