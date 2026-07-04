using Funda.DAL.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace Funda.DAL.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDalServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddHttpClients(services, configuration);

            return services;
        }

        private static IServiceCollection AddHttpClients(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IFundaObjectsClient, FundaObjectsClient>(client =>
            {
                string fundaBaseUrl = configuration?.GetValue<string>("Funda:BaseUrl");
                string fundaUrlKey = configuration?.GetValue<string>("Funda:ApiKey");

                client.BaseAddress = new Uri($"{fundaBaseUrl}/{fundaUrlKey}");
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            // Add the standard resilience pipeline (Includes Retry, Circuit Breaker, Rate Limiting, Timeout)
            .AddStandardResilienceHandler(options =>
            {
                // Create your time-based window limiter (max number of requests per minute; e.g. 100)
                // Use to prevent hitting the servers limit
                var fixedWindowLimiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
                {
                    PermitLimit = configuration?.GetValue<int?>("Funda:MaxNumberOfRequestsPerMinuteForPropertiesLookup") ?? 100, // Limit to number of requests (configurable) per minute to avoid hitting the server's wall (e.g. >100 per minute).
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 20, // Internal queue for bursts after the limit is reached (configurable)
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });

                // Customize the Rate Limiter
                // Override the default concurrency limiter with your time-based window limiter
                options.RateLimiter.RateLimiter = args =>
                    fixedWindowLimiter.AcquireAsync(permitCount: 1, args.Context.CancellationToken);
                
                // Customize Retries for transient errors (429, 5xx, or network drops)
                // Should handle those error codes out of the box
                // Exponential backoff: waits 2s, then 4s, then 8s...
                options.Retry.MaxRetryAttempts = configuration?.GetValue<int?>("Funda:MaxRetryAttempts") ?? 3;
                options.Retry.Delay = TimeSpan.FromSeconds(2);
                options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
            });

            return services;
        }
    }
}
