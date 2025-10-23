using NetBuild.Domain.Entities;
using System.Linq.Expressions;

namespace NetBuild.Domain.Repositories
{
    public interface IBuildingRepository
    {
        Task<Building> AddAsync(Building entity);
        Task DeleteAsync(string id, string neighborhoodId);
        Task<IEnumerable<Building>> GetAsync(Expression<Func<Building, bool>>? predicate = null);
        Task<Building?> GetByIdAsync(string id, string neighborhoodId);
        Task UpdateAsync(Building entity);
        Task PatchAsync(string id, string neighborhoodId, Dictionary<string, object> fieldsToUpdate);
    }
}
