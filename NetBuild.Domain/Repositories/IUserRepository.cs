using NetBuild.Domain.Entities;
using System.Linq.Expressions;

namespace NetBuild.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<string> AddAsync(User entity);
        Task DeleteAsync(string id);
        Task<IEnumerable<User>> GetAsync(Expression<Func<User, bool>>? predicate = null);
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateAsync(User entity);
    }
}