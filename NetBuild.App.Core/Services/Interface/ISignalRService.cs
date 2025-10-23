using NetBuild.App.Core.ApiModel;
using System.Security.Claims;

namespace NetBuild.App.Core.Services.Interface
{
    public interface ISignalRService : IDisposable
    {
        Task<SignalRConnectionInfo> NegotiateAsync(string userId, IList<Claim> claims);
        Task SendAsync(string groupId, string target, object message);
        Task AddToGroupAsync(string connectionId, string groupId);
        Task AddToGroupsAsync(string connectionId, List<string> groups);
        Task RemoveFromGroupAsync(string connectionId, string groupId);
        Task RemoveFromGroupsAsync(string connectionId, List<string> groups);
    }
}
