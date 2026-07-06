using Funda.Application.DI;
using Funda.DAL.DI;
using Funda.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Serilog;

namespace Funda.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenApi();
            services.AddOptions<FundaConfig>()
                .Bind(configuration.GetSection(FundaConfig.SectionName))
                .ValidateOnStart(); // Ensures configuration is bound properly on startup

            services.AddApplicationServices()
                    .AddDalServices(configuration)
                    .ConfigureHybridCache(configuration)
                    .ConfigureLogger(configuration);

            return services;
        }

        private static IServiceCollection ConfigureLogger(this IServiceCollection services, IConfiguration configuration)
        {
            // This automatically reads configuration from your appsettings.json
            services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console());

            return services;
        }

        private static IServiceCollection ConfigureHybridCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHybridCache(options =>
            {
                int cacheDurationMinutes = configuration.GetValue<int>("InMemoryCacheDurationInMinutes", defaultValue: 5);

                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(cacheDurationMinutes) // Cache duration
                };

                // Safety guard: Don't allow massive payloads to crash your server's RAM
                options.MaximumPayloadBytes = 1024 * 1024; // 1 MB per entry max
            });

            return services;
        }
    }
}
