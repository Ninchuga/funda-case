using Funda.DAL.Models;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using Funda.Shared.Extensions;
using Funda.DAL.Extensions;

namespace Funda.DAL.Clients
{
    internal class FundaPropertiesClient : IFundaPropertiesClient
    {
        private readonly HttpClient _httpClient;

        public FundaPropertiesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(List<Makelaar> makelaars, int totalPages)> GetPropertiesMakellars(string city, bool propertiesWithGarden, PropertyType propertyType, int page = 1, int pageSize = 100)
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
                    var properties = await response.Content.ReadFromJsonAsync<FundaPropertiesToSellModel>();

                    return (properties.ToMakelaars(), properties?.Paging.TotalPages ?? 0);
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
