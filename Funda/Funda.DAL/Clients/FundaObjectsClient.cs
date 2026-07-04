using Funda.DAL.Models;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using System.Net.Http.Json;
using Funda.Shared.Extensions;
using Funda.DAL.Extensions;

namespace Funda.DAL.Clients
{
    internal class FundaObjectsClient : IFundaObjectsClient
    {
        private readonly HttpClient _httpClient;

        public FundaObjectsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(List<Makelaar> makelaars, int totalPages)> GetMakellarsDataFromObjects(string city, bool propertiesWithGarden, PropertyType propertyType, int page = 1, int pageSize = 25)
        {
            // Query parameters need to be lower case! Otherwise 401 unauthorized error is returned from the api...missleading/unrelated error
            string urlPath = propertiesWithGarden
                ? $"?type={propertyType.ToLowerString()}&zo=/{city.ToLower()}/tuin/&page={page}&pageSize={pageSize}"
                : $"?type={propertyType.ToLowerString()}&zo=/{city.ToLower()}/&page={page}&pageSize={pageSize}";
            string url = $"{_httpClient.BaseAddress}/{urlPath}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var objectsResponse = await response.Content.ReadFromJsonAsync<FundaObjectsResponseModel>();

                    return (objectsResponse.ToMakelaars(), objectsResponse?.Paging.TotalPages ?? 0);
                }
            }
            catch (Exception ex)
            {
                // TODO: log error here
            }

            return ([], 0);
        }
    }
}
