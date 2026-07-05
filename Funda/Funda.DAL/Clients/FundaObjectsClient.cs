using Funda.DAL.Extensions;
using Funda.DAL.Models;
using Funda.Domain.Models;
using Funda.Shared.Extensions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Funda.DAL.Clients
{
    internal class FundaObjectsClient(HttpClient httpClient, ILogger<FundaObjectsClient> logger) : IFundaObjectsClient
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<FundaObjectsClient> _logger = logger;

        public async Task<Result<GetMakelaarsDataFromObjectsResponse>> GetMakellarsDataFromObjects(GetMakelaarsDataFromObjectsRequest request, CancellationToken cancellationToken)
        {
            // If too many requests are made to the api (>100 per minute), it will return 401 Unauthorized error...missleading/unrelated error.
            // Query parameters need to be lower case! Otherwise 401 unauthorized error is returned from the api...missleading/unrelated error.
            string urlPath = request.PropertiesWithGarden
                ? $"?type={request.PropertyType.ToLowerString()}&zo=/{request.City.ToLower()}/tuin/&page={request.PageNumber}&pageSize={request.PageSize}"
                : $"?type={request.PropertyType.ToLowerString()}&zo=/{request.City.ToLower()}/&page={request.PageNumber}&pageSize={request.PageSize}";
            string url = $"{_httpClient.BaseAddress}/{urlPath}";

            try
            {
                var getObjectsResponse = await _httpClient.GetAsync(url, cancellationToken);

                if (!getObjectsResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Request was not executed successfully for city: {City}, propertiesWithGarden: {PropertiesWithGarden}, propertyType: {PropertyType}, page: {Page}, pageSize: {PageSize}. Response status code: {StatusCode}",
                        request.City, request.PropertiesWithGarden, request.PropertyType, request.PageNumber, request.PageSize, getObjectsResponse.StatusCode);

                    return Result<GetMakelaarsDataFromObjectsResponse>.Failure($"Failed to fetch makelaars data from Funda API. Response status code: {getObjectsResponse.StatusCode}");
                }

                var objectsResponse = await getObjectsResponse.Content.ReadFromJsonAsync<FundaObjectsResponseModel>();

                var response = new GetMakelaarsDataFromObjectsResponse(
                    Makelaars: objectsResponse.ToMakelaars().AsReadOnly(),
                    TotalPages: objectsResponse?.Paging.TotalPages ?? 0,
                    TotalObjects: objectsResponse?.TotalNumberOfObjects ?? 0);
                return Result<GetMakelaarsDataFromObjectsResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while fetching makelaars data from Funda API for city: {City}, propertiesWithGarden: {PropertiesWithGarden}, propertyType: {PropertyType}, page: {Page}, pageSize: {PageSize}. Error message: {ErrorMessage}",
                    request.City, request.PropertiesWithGarden, request.PropertyType, request.PageNumber, request.PageSize, ex.Message);
            }

            return Result<GetMakelaarsDataFromObjectsResponse>.Failure($"Failed to fetch makelaars data from Funda API.");
        }
    }
}
