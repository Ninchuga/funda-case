using Funda.DAL.Models;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

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
        public async Task<List<Makelaar>> GetPropertiesMakellars(string city, bool propertiesWithGarden, PropertyType propertyType)
        {
            string urlPath = propertiesWithGarden ? $"?type={propertyType}&zo=/{city}/tuin/" : $"?type={propertyType}&zo=/{city}/";
            string url = $"{_httpClient.BaseAddress}/{urlPath}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                //var response = await _httpClient.GetStringAsync(url);
                //var properties = JsonConvert.DeserializeObject<FundaPropertiesToSellModel>(response);
                //var properties = JsonConvert.DeserializeObject<FundaPropertiesToSellModel>(response.Content.ReadFromJsonAsync<FundaPropertiesToSellModel>());
                var properties = await response.Content.ReadFromJsonAsync<FundaPropertiesToSellModel>();

                if (response.IsSuccessStatusCode)
                {
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
