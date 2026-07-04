using Funda.DAL.Models;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using Funda.Shared.Extensions;

namespace Funda.DAL.Clients
{
    internal class FundaPropertiesClient : IFundaPropertiesClient
    {
        private readonly HttpClient _httpClient;

        public FundaPropertiesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // TODO: Add logic to get the makelaars for the given city, propertiesWithGarden and propertyType
        // implement retry and error handling logic and count top ten makelaars based on the number of properties they have listed in the given city,
        // with the given property type and garden preference.
        // Add caching to avoid hitting the API too often and to improve performance.

        // This method should use pagination and max number of properties per call
        // Make it configurable
        public async Task<List<Makelaar>> GetPropertiesMakellars(string city, bool propertiesWithGarden, PropertyType propertyType, int page = 1, int pageSize = 1000)
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

                    return []; // TODO: Map this to domain model Makelaar and return the list of Makelaars
                }
            }
            catch (Exception ex)
            {
                // TODO: log error here
            }

            return [];
        }
    }
}
