using Funda.Application.Extensions;
using Funda.Application.Models;
using Funda.DAL.Clients;
using Funda.DAL.Models;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using Funda.Shared.Configurations;
using Funda.Shared.Constants;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Funda.Application.Services
{
    internal class MakelaarService(IFundaObjectsClient fundaPropertiesClient, IOptionsSnapshot<FundaConfig> fundaConfig, HybridCache cache, ILogger<MakelaarService> logger) : IMakelaarService
    {
        private readonly IFundaObjectsClient _fundaPropertiesClient = fundaPropertiesClient;
        private readonly FundaConfig _fundaConfig = fundaConfig.Value;
        private readonly HybridCache _cache = cache;
        private readonly ILogger<MakelaarService> _logger = logger;

        public async Task<Result<List<SellingMakelaarDto>>> GetTopTenSellingMakelaarsFor(string city, bool propertiesWithGarden, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executing {ServiceName}.{MethodName} city: {City}, propertiesWithGarden: {PropertiesWithGarden}, propertyType: {PropertyType}....",
                nameof(MakelaarService), nameof(GetTopTenSellingMakelaarsFor), city, propertiesWithGarden, PropertyType.Koop);

            string cacheKey = $"TopTenSellingMakelaars_{city}_garden:{propertiesWithGarden}";
            var topTenMakelaarsCache = await TryGetTopSellingMakelaarsFromCache(cacheKey, cancellationToken);
            if (topTenMakelaarsCache is not null)
                return Result<List<SellingMakelaarDto>>.Success(topTenMakelaarsCache);

            var topTenMakelaarsResult = await GetTopTenSellingMakelaars(city, propertiesWithGarden, cancellationToken);
            if (!topTenMakelaarsResult.IsSuccess)
                return topTenMakelaarsResult;

            await _cache.SetAsync(cacheKey, topTenMakelaarsResult.Data, cancellationToken: cancellationToken);

            return Result<List<SellingMakelaarDto>>.Success(topTenMakelaarsResult.Data);
        }

        private async Task<List<SellingMakelaarDto>?> TryGetTopSellingMakelaarsFromCache(string cacheKey, CancellationToken cancellationToken)
        {
            return await _cache.GetOrCreateAsync<List<SellingMakelaarDto>>(
                            cacheKey,
                            factory: null, // Passing null because we disable underlying data execution flags, just checking the cache
                            options: new HybridCacheEntryOptions
                            {
                                Flags = HybridCacheEntryFlags.DisableUnderlyingData
                            },
                            cancellationToken: cancellationToken);
        }

        private async Task<Result<List<SellingMakelaarDto>>> GetTopTenSellingMakelaars(string city, bool propertiesWithGarden, CancellationToken cancellationToken)
        {
            int pageSize = _fundaConfig?.ObjectsLookupApi?.PageSize ?? FundaApplicationConstants.ApiConfig.PageSize;
            int currentPage = 1;
            int totalPages = 1;

            Dictionary<int, Makelaar> makelaarPropertiesForSale = [];

            var request = new GetMakelaarsDataFromObjectsRequest(
                    City: city,
                    PropertiesWithGarden: propertiesWithGarden,
                    PropertyType: PropertyType.Koop,
                    PageNumber: currentPage,
                    PageSize: pageSize
                );

            while (currentPage <= totalPages)
            {
                var makelaarsResponse = await _fundaPropertiesClient.GetMakellarsDataFromObjects(request, cancellationToken);
                if (!makelaarsResponse.IsSuccess)
                    return Result<List<SellingMakelaarDto>>.Failure(makelaarsResponse.Errors);

                if (makelaarsResponse.Data?.Makelaars is null || makelaarsResponse.Data.Makelaars.Count == 0)
                    break;

                if (currentPage == 1) // Only set totalPages on the first iteration
                    totalPages = makelaarsResponse.Data.TotalPages;

                SyncMakelaarsPropertiesForSale(makelaarPropertiesForSale, makelaarsResponse.Data.Makelaars);

                currentPage++;
                request = request with { PageNumber = currentPage };
            }

            var topTenMakelaars = makelaarPropertiesForSale.OrderByDescending(x => x.Value.NumberOfPropertiesForSale).ThenBy(x => x.Value.Id).Take(10);

            _logger.LogInformation("Executed {ServiceName}.{MethodName}. Last page executed: {currentPage}", nameof(MakelaarService), nameof(GetTopTenSellingMakelaarsFor), currentPage);

            return Result<List<SellingMakelaarDto>>.Success([.. topTenMakelaars.Select(x => x.Value.ToDto())]);
        }

        private static void SyncMakelaarsPropertiesForSale(Dictionary<int, Makelaar> makelaarPropertiesForSale, IEnumerable<Makelaar> makelaars)
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
                    makelaarPropertiesForSale[makelaar.Id] = makelaarPropertiesForSale[makelaar.Id] with
                    {
                        NumberOfPropertiesForSale = value.NumberOfPropertiesForSale + makelaarGroup.Count()
                    };
                }
            }
        }
    }
}