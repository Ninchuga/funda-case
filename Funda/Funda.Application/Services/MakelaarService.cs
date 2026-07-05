using Funda.Application.Extensions;
using Funda.Application.Models;
using Funda.DAL.Clients;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using Funda.Shared.Configurations;
using Funda.Shared.Constants;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Funda.Application.Services
{
    internal class MakelaarService : IMakelaarService
    {
        private readonly IFundaObjectsClient _fundaPropertiesClient;
        private readonly FundaConfig _fundaConfig;
        private readonly HybridCache _cache;
        private readonly ILogger<MakelaarService> _logger;

        public MakelaarService(IFundaObjectsClient fundaPropertiesClient, IOptionsSnapshot<FundaConfig> fundaConfig, HybridCache cache, ILogger<MakelaarService> logger)
        {
            _fundaPropertiesClient = fundaPropertiesClient;
            _fundaConfig = fundaConfig.Value;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<SellingMakelaarDto>> GetTopTenSellingMakelaarsFor(string city, bool propertiesWithGarden)
        {
            string cacheKey = $"TopTenSellingMakelaars_{city}_{propertiesWithGarden}";

            //var topTenMakelaars = await _cache.GetOrCreateAsync(
            //    cacheKey,
            //    async token => await FetchProductFromDbAsync(id, token),
            //    cancellationToken: ct);

            int pageSize = _fundaConfig?.ObjectsLookupApi?.PageSize ?? FundaApplicationConstants.ApiConfig.PageSize;
            int currentPage = 1;
            int totalPages = 1;

            _logger.LogInformation("Executing {ServiceName}.{MethodName} city: {City}, propertiesWithGarden: {PropertiesWithGarden}, propertyType: {PropertyType}....", 
                nameof(MakelaarService), nameof(GetTopTenSellingMakelaarsFor), city, propertiesWithGarden, PropertyType.Koop);

            Dictionary<int, Makelaar> makelaarPropertiesForSale = [];

            while (currentPage <= totalPages)
            {
                var makelaarsResponse = await _fundaPropertiesClient.GetMakellarsDataFromObjects(city, propertiesWithGarden, PropertyType.Koop, currentPage, pageSize);
                if (makelaarsResponse.makelaars is null || makelaarsResponse.makelaars.Count == 0)
                    break;

                if (currentPage == 1) // Only set totalPages on the first iteration
                    totalPages = makelaarsResponse.totalPages;

                SyncMakelaarsPropertiesForSale(makelaarPropertiesForSale, makelaarsResponse.makelaars);

                currentPage++;
            }

            var topTenMakelaars = makelaarPropertiesForSale.OrderByDescending(x => x.Value.NumberOfPropertiesForSale).ThenBy(x => x.Value.Id).Take(10);

            _logger.LogInformation("Executed {ServiceName}.{MethodName}. Last page executed: {currentPage}", nameof(MakelaarService), nameof(GetTopTenSellingMakelaarsFor), currentPage);

            // TODO: Add topTenMakelaars to a cache to avoid unneccessary calls to the Funda API. The cache should be invalidated after a certain period of time or when the data changes.

            return [.. topTenMakelaars.Select(x => x.Value.ToDto())];
        }

        private static void SyncMakelaarsPropertiesForSale(Dictionary<int, Makelaar> makelaarPropertiesForSale, List<Makelaar> makelaars)
        {
            var group = makelaars.GroupBy(m => m.Id);
            foreach (var makelaarGroup in group)
            {
                var makelaar = makelaarGroup.First();
                if (!makelaarPropertiesForSale.TryGetValue(makelaar.Id, out Makelaar? value))
                {
                    makelaarPropertiesForSale[makelaar.Id] = new Makelaar(makelaar.Id, makelaar.Name, NumberOfPropertiesForSale: makelaarGroup.Count());
                }
                else
                {
                    makelaarPropertiesForSale[makelaar.Id] = new Makelaar(
                        makelaar.Id,
                        makelaar.Name,
                        NumberOfPropertiesForSale: value.NumberOfPropertiesForSale + makelaarGroup.Count()
                    );
                }
            }
        }
    }
}
