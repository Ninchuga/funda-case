using Funda.DAL.Clients;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Funda.Application.Services
{
    internal class MakelaarService : IMakelaarService
    {
        private readonly IFundaPropertiesClient _fundaPropertiesClient;
        private readonly IConfiguration _configuration;

        public MakelaarService(IFundaPropertiesClient fundaPropertiesClient, IConfiguration configuration)
        {
            _fundaPropertiesClient = fundaPropertiesClient;
            _configuration = configuration;
        }

        public async Task<List<Makelaar>> GetMakelaarsFor(string city, bool propertiesWithGarden, PropertyType propertyType)
        {
            int pageSize = _configuration?.GetValue<int?>("Funda:GetPropertiesPageSize") ?? 25;
            int currentPage = 1;
            int totalPages = 1;

            while (currentPage <= totalPages)
            {
                var makelaarsResponse = await _fundaPropertiesClient.GetPropertiesMakellars(city, propertiesWithGarden, propertyType, currentPage, pageSize);
                if (makelaarsResponse.makelaars is null || makelaarsResponse.makelaars.Count == 0)
                    break;

                if (currentPage == 1) // Only set totalPages on the first iteration
                    totalPages = makelaarsResponse.totalPages;

                currentPage++;
            }

            return [];
        }
    }
}
