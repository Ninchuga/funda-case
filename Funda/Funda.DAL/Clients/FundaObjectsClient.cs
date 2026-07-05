using Funda.DAL.Extensions;
using Funda.DAL.Models;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using Funda.Shared.Extensions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Funda.DAL.Clients
{
    internal class FundaObjectsClient : IFundaObjectsClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FundaObjectsClient> _logger;

        public FundaObjectsClient(HttpClient httpClient, ILogger<FundaObjectsClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(List<Makelaar> makelaars, int totalPages)> GetMakellarsDataFromObjects(string city, bool propertiesWithGarden, PropertyType propertyType, int page = 1, int pageSize = 25)
        {
            // If too many requests are made to the api (>100 per minute), it will return 401 Unauthorized error...missleading/unrelated error.
            // Query parameters need to be lower case! Otherwise 401 unauthorized error is returned from the api...missleading/unrelated error.
            string urlPath = propertiesWithGarden
                ? $"?type={propertyType.ToLowerString()}&zo=/{city.ToLower()}/tuin/&page={page}&pageSize={pageSize}"
                : $"?type={propertyType.ToLowerString()}&zo=/{city.ToLower()}/&page={page}&pageSize={pageSize}";
            string url = $"{_httpClient.BaseAddress}/{urlPath}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Request was not executed successfully for city: {City}, propertiesWithGarden: {PropertiesWithGarden}, propertyType: {PropertyType}, page: {Page}, pageSize: {PageSize}. Response status code: {StatusCode}", city, propertiesWithGarden, propertyType, page, pageSize, response.StatusCode);
                    return ([], 0);
                }

               var objectsResponse = await response.Content.ReadFromJsonAsync<FundaObjectsResponseModel>();
               return (objectsResponse.ToMakelaars(), objectsResponse?.Paging.TotalPages ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while fetching makelaars data from Funda API for city: {City}, propertiesWithGarden: {PropertiesWithGarden}, propertyType: {PropertyType}, page: {Page}, pageSize: {PageSize}. Error message: {ErrorMessage}", city, propertiesWithGarden, propertyType, page, pageSize, ex.Message);
            }

            return ([], 0);
        }
    }
}
