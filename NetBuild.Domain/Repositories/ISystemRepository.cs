using System.Linq.Expressions;

namespace NetBuild.Domain.Repositories
{
    public interface ISystemRepository
    {
        Task<Entities.System> AddAsync(Entities.System entity);
        Task DeleteAsync(string id, string buildingId);
        Task<IEnumerable<Entities.System>> GetAsync(Expression<Func<Entities.System, bool>>? predicate = null);
        Task<Entities.System?> GetByIdAsync(string id, string buildingId);
        Task UpdateAsync(Entities.System entity);
    }
}
