using Funda.DAL.Clients;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Funda.Application.Services
{
    internal class MakelaarService : IMakelaarService
    {
        public IFundaPropertiesClient _fundaPropertiesClient;

        public MakelaarService(IFundaPropertiesClient fundaPropertiesClient)
        {
            _fundaPropertiesClient = fundaPropertiesClient;
        }

        public async Task<List<Makelaar>> GetMakelaarsFor(string city, bool propertiesWithGarden, PropertyType propertyType, int page = 1, int pageSize = 1000)
        {
            
            var makelaars = _fundaPropertiesClient.GetPropertiesMakellars(city, propertiesWithGarden, propertyType, page, pageSize);

            return [];
        }
    }
}
