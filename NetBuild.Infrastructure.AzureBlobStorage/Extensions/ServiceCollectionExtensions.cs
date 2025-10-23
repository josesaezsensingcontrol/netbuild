using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetBuild.Domain.Managers;
using NetBuild.Infrastructure.AzureBlobStorage.Configuration;

namespace NetBuild.Infrastructure.AzureBlobStorage.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureBlobStorageOption>(configuration.GetSection("AzureBlobStorageOption"));
            services.AddSingleton<IBlobStorageManager, AzureBlobStorageManager>();

            return services;
        }
    }
}
