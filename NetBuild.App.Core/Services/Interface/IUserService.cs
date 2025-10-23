using NetBuild.App.Core.ApiModel;
using NetBuild.Domain.Types;

namespace NetBuild.App.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string userId);
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync(string? parentId, UserRole? role);
        Task<bool> ExistsUserAsync(string email);
    }
}
