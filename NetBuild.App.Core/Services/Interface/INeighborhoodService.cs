using NetBuild.App.Core.ApiModel;

namespace NetBuild.App.Core.Services.Interface
{
    public interface INeighborhoodService
    {
        Task<Neighborhood?> AddNeighborhoodAsync(string name, string description);
        
        Task<Neighborhood?> GetNeighborhoodAsync(string id);

        Task<IEnumerable<Neighborhood>> GetAllNeighborhoodsAsync();

        Task UpdateNeighborhoodAsync(string id, string name, string description);

        Task DeleteNeighborhoodAsync(string id);
    }
}
