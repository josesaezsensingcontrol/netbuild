using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NetBuild.App.Core.ApiModel;
using NetBuild.App.Core.Configuration;
using NetBuild.App.Core.Services.Interface;
using System.Security.Claims;

namespace NetBuild.App.Core.Services
{
    public class SignalRService : ISignalRService
    {
        private const string NetBuildHubName = "netBuildHub";

        public readonly ServiceHubContext _netBuildHub;

        public SignalRService(IOptions<SignalRConfiguration> options)
        {
            using var serviceManager = new ServiceManagerBuilder()
                .WithOptions(option =>
                {
                    option.ConnectionString = options.Value.ConnectionString;
                })
                .WithNewtonsoftJson(option => {
                    option.PayloadSerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver {
                            NamingStrategy = new CamelCaseNamingStrategy
                            {
                                ProcessDictionaryKeys = false
                            }
                        }
                    };
                })
                .BuildServiceManager();

            _netBuildHub = serviceManager.CreateHubContextAsync(NetBuildHubName, new CancellationToken()).GetAwaiter().GetResult();
        }

        public async Task<SignalRConnectionInfo> NegotiateAsync(string userId, IList<Claim> claims)
        {
            var negotiateResponse = await _netBuildHub.NegotiateAsync(new() { UserId = userId, Claims = claims });

            return new SignalRConnectionInfo { Url = negotiateResponse.Url, AccessToken = negotiateResponse.AccessToken };
        }

        public async Task SendAsync(string groupId, string target, object message)
        {
            await _netBuildHub.Clients.Group(groupId).SendCoreAsync(target, new[] { message });
        }

        public async Task AddToGroupAsync(string connectionId, string groupId)
        {
            await _netBuildHub.Groups.AddToGroupAsync(connectionId, groupId);
        }

        public async Task AddToGroupsAsync(string connectionId, List<string> groups)
        {
            await Parallel.ForEachAsync(groups, async (group, token) =>
            {
                await _netBuildHub.Groups.AddToGroupAsync(connectionId, group);
            });
        }

        public async Task RemoveFromGroupAsync(string connectionId, string groupId)
        {
            await _netBuildHub.Groups.RemoveFromGroupAsync(connectionId, groupId);
        }

        public async Task RemoveFromGroupsAsync(string connectionId, List<string> groups)
        {
            await Parallel.ForEachAsync(groups, async (group, token) =>
            {
                await _netBuildHub.Groups.RemoveFromGroupAsync(connectionId, group);
            });
        }

        public void Dispose()
        {
            _netBuildHub.Dispose();
        }
    }
}
