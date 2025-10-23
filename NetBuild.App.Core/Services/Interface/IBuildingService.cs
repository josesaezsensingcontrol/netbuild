using NetBuild.App.Core.ApiModel;

namespace NetBuild.App.Core.Services.Interface
{
    public interface IBuildingService
    {
        Task<bool> CanSeeBuildingAsync(JwtData userData, string neighborhoodId, string buildingId);

        Task<bool> CanEditBuildingAsync(JwtData userData, string neighborhoodId, string buildingId);

        Task<Building?> AddBuildingAsync(string neighborhoodId, string ownerId, string name, string description, double latitude, double longitude);

        Task<Building?> GetBuildingAsync(string buildingId, string neighborhoodId);

        Task<IEnumerable<Building>> GetAllBuildingsAsync(string? neighborhoodId = null, string? ownerId = null);

        Task UpdateBuildingAsync(string buildingId, string neighborhoodId, string name, string description, double latitude, double longitude);

        Task UpdateBuildingScheduleAsync(string buildingId, string neighborhoodId, Schedule schedule);

        Task UpdateBuildingDiagramImageAsync(string buildingId, string neighborhoodId, string imageUrl);

        Task UpdateBuildingDiagramNodesAsync(string buildingId, string neighborhoodId, IEnumerable<DiagramNode> nodes);

        Task UpdateBuildingWeatherForecastAsync(string buildingId, string neighborhoodId, WeatherForecast nodes);

        Task DeleteBuildingAsync(string buildingId, string neighborhoodId);
    }
}
