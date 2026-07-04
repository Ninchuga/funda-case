using Funda.Application.Extensions;
using Funda.Application.Models;
using Funda.DAL.Clients;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Funda.Application.Services
{
    internal class MakelaarService : IMakelaarService
    {
        private readonly IFundaObjectsClient _fundaPropertiesClient;
        private readonly IConfiguration _configuration;

        public MakelaarService(IFundaObjectsClient fundaPropertiesClient, IConfiguration configuration)
        {
            _fundaPropertiesClient = fundaPropertiesClient;
            _configuration = configuration;
        }

        public async Task<List<SellingMakelaarDto>> GetTopTenSellingMakelaarsFor(string city, bool propertiesWithGarden)
        {
            int pageSize = _configuration?.GetValue<int?>("Funda:GetPropertiesPageSize") ?? 25;
            int currentPage = 1;
            int totalPages = 1;

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
