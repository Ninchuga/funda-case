using Funda.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Funda.Tests.Fixtures
{
    public class MakelaarServiceTestFixture
    {
        public const string DefaultCity = "Amsterdam";
        public HybridCache Cache;

        public MakelaarServiceTestFixture()
        {
            var services = new ServiceCollection();

            services.AddLogging(); // Add logging (required internally by HybridCache)
            services.AddHybridCache();

            var serviceProvider = services.BuildServiceProvider();

            Cache = serviceProvider.GetRequiredService<HybridCache>();
        }

        public IOptionsSnapshot<FundaConfig> SetupFundaConfigMock()
        {
            var mockSnapshot = new Mock<IOptionsSnapshot<FundaConfig>>();
            var fundaConfig = new FundaConfig
            {
                ObjectsLookupApi = new ObjectsLookupApiConfig
                {
                    MaxNumberOfRequestsPerMinute = 100,
                    MaxRetryAttempts = 5,
                    PageSize = 25
                }
            };
            mockSnapshot.Setup(x => x.Value).Returns(fundaConfig);

            return mockSnapshot.Object;
        }

        public async Task SetCacheKey<T>(string cacheKey, T value) =>
            await Cache.SetAsync(cacheKey, value);

        public async Task RemoveCacheKey(string cacheKey) =>
            await Cache.RemoveAsync(cacheKey);
    }
}
