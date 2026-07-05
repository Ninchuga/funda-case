using Funda.DAL.Clients;
using Funda.Shared.Configurations;
using Funda.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using System.Net;
using System.Threading.RateLimiting;

namespace Funda.DAL.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddDalServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddHttpClients(services, configuration);

        return services;
    }

    private static IServiceCollection AddHttpClients(IServiceCollection services, IConfiguration configuration)
    {
        var fundaConfig = configuration?.GetSection(FundaConfig.SectionName).Get<FundaConfig>();

        services.AddHttpClient<IFundaObjectsClient, FundaObjectsClient>(client =>
        {
            string fundaBaseUrl = fundaConfig?.BaseUrl;
            string fundaApiKey = fundaConfig?.ApiKey;

            client.BaseAddress = new Uri($"{fundaBaseUrl}/{fundaApiKey}");
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        })
        // Add the standard resilience pipeline (Includes Retry, Circuit Breaker, Rate Limiting, Timeout)
        .AddStandardResilienceHandler(options =>
        {
            ConfigureRateLimiter(fundaConfig, options);
            ConfigureRetries(fundaConfig, options);
        });

        return services;
    }

    private static void ConfigureRateLimiter(FundaConfig fundaConfig, HttpStandardResilienceOptions options)
    {
        // Create your time-based window limiter (max number of requests per minute; e.g. 100)
        // Use to prevent hitting the servers limit
        var fixedWindowLimiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = fundaConfig?.ObjectsLookupApi?.MaxNumberOfRequestsPerMinute ?? FundaApplicationConstants.ApiConfig.MaxNumberOfRequestsPerMinute, // Limit to number of requests (configurable) per minute to avoid hitting the server's wall (e.g. >100 per minute).
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 20, // Internal queue for bursts after the limit is reached (configurable)
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });

        // Customize the Rate Limiter
        // Override the default concurrency limiter with your time-based window limiter
        options.RateLimiter.RateLimiter = args =>
            fixedWindowLimiter.AcquireAsync(permitCount: 1, args.Context.CancellationToken);
    }

    private static void ConfigureRetries(FundaConfig fundaConfig, HttpStandardResilienceOptions options)
    {
        // Combine default transient handling with your custom 401 handling
        options.Retry.ShouldHandle = args =>
        {
            var isTransientOrUnauthorized = args.Outcome switch
            {
                // 1. Handles network drops / connection failures
                { Exception: HttpRequestException } => true,

                // 2. Handles 408 Request Timeout
                { Result.StatusCode: HttpStatusCode.RequestTimeout } => true,

                // 3. Handles 5xx Internal Server Errors
                { Result.StatusCode: >= HttpStatusCode.InternalServerError } => true,

                // 4. Your custom requirement: Handle 401 Unauthorized
                // Added to handle 401 Unauthorized responses.
                // This is only for this specific api as this error is missleading and it will returned in case of rate limiting.
                // This rate limiting error code from funda objects lookup api should be changed to 429
                { Result.StatusCode: HttpStatusCode.Unauthorized } => true, 

                // Don't retry on anything else
                _ => false
            };

            return ValueTask.FromResult(isTransientOrUnauthorized);
        };

        // Customize Retries for transient errors (429, 5xx, or network drops)
        // Should handle those error codes out of the box
        // Exponential backoff: waits 2s, then 4s, then 8s...
        options.Retry.MaxRetryAttempts = fundaConfig?.ObjectsLookupApi?.MaxRetryAttempts ?? FundaApplicationConstants.ApiConfig.MaxRetries;
        options.Retry.Delay = TimeSpan.FromSeconds(2);
        options.Retry.BackoffType = DelayBackoffType.Exponential;
    }
}