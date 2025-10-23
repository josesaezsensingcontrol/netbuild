using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetBuild.Domain.Repositories;
using NetBuild.Infrastructure.CosmosDb.Configuration;
using NetBuild.Infrastructure.CosmosDb.Repositories;

namespace NetBuild.Infrastructure.CosmosDb.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfiguration configuration)
        {
            var cosmosDbConfig = new CosmosDbConfiguration();
            configuration.GetSection("CosmosDbOption").Bind(cosmosDbConfig);

            var cosmosContainersConfig = new CosmosContainersConfiguration();
            configuration.GetSection("CosmosContainersOption").Bind(cosmosContainersConfig);

            services.Configure<CosmosDbConfiguration>(configuration.GetSection("CosmosDbOption"));
            services.Configure<CosmosContainersConfiguration>(configuration.GetSection("CosmosContainersOption"));

            services.AddSingleton(InitializeCosmosClientInstanceAsync(cosmosDbConfig, cosmosContainersConfig).GetAwaiter().GetResult());

            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<INeighborhoodRepository, NeighborhoodRepository>();
            services.AddSingleton<IBuildingRepository, BuildingRepository>();
            services.AddSingleton<ISystemRepository, SystemRepository>();
            services.AddSingleton<IDataRepository, DataRepository>();

            return services;
        }

        private static async Task<CosmosClient> InitializeCosmosClientInstanceAsync(CosmosDbConfiguration config, CosmosContainersConfiguration containersConfig)
        {
            var options = new CosmosClientOptions()
            {
                AllowBulkExecution = false,
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };

            CosmosClient client = new CosmosClient(config.AccountEndpoint, config.AccountKey, options);

            if (config.CreateIfNotExistsOnStartup)
            {
                DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(config.DatabaseName);
                await database.Database.CreateContainerIfNotExistsAsync(containersConfig.UserContainerName, "/id");
                await database.Database.CreateContainerIfNotExistsAsync(containersConfig.NeighborhoodContainerName, "/id");
                await database.Database.CreateContainerIfNotExistsAsync(containersConfig.BuildingContainerName, "/neighborhoodId");
                await database.Database.CreateContainerIfNotExistsAsync(containersConfig.SystemContainerName, "/buildingId");
                await database.Database.CreateContainerIfNotExistsAsync(containersConfig.HistoricDataContainerName, "/dataId");
                await database.Database.CreateContainerIfNotExistsAsync(containersConfig.PredictionDataContainerName, "/dataId");
            }

            return client;
        }
    }
}
