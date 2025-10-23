using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetBuild.App.Core.Configuration;
using NetBuild.App.Core.Mappers;
using NetBuild.App.Core.Services;
using NetBuild.App.Core.Services.Interface;
using NetBuild.App.Core.Services.Interfaces;
using NetBuild.App.Core.Weather;
using SendGrid.Extensions.DependencyInjection;

namespace NetBuild.App.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthenticationConfiguration>(configuration.GetSection("AuthenticationOption"));
            services.Configure<ClientAuthConfiguration>(configuration.GetSection("ClientAuthOption"));
            services.Configure<SendGridConfiguration>(configuration.GetSection("SendGridOption"));
            services.Configure<SignalRConfiguration>(configuration.GetSection("SignalROption"));

            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<INeighborhoodService, NeighborhoodService>();
            services.AddSingleton<IBuildingService, BuildingService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<IMailService, SendGridMailService>();
            services.AddSingleton<ISignalRService, SignalRService>();

            services.AddAutoMapper(typeof(AutoMapping));
            services.AddSendGrid((serviceProvider, options) =>
            {
                var sendGridOption = serviceProvider.GetRequiredService<IOptions<SendGridConfiguration>>();
                options.ApiKey = sendGridOption.Value.ApiKey;
            });
            services.AddSingleton<OpenMeteoClient>();

            return services;
        }
    }
}
