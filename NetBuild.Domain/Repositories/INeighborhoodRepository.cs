using NetBuild.Domain.Entities;
using System.Linq.Expressions;

namespace NetBuild.Domain.Repositories
{
    public interface INeighborhoodRepository
    {
        Task<Neighborhood> AddAsync(Neighborhood entity);
        Task DeleteAsync(string id);
        Task<IEnumerable<Neighborhood>> GetAsync(Expression<Func<Neighborhood, bool>>? predicate = null);
        Task<Neighborhood?> GetByIdAsync(string id);
        Task UpdateAsync(Neighborhood entity);
        Task PatchAsync(string id, Dictionary<string, object> fieldsToUpdate);
    }
}
