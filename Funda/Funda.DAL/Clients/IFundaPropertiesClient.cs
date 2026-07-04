using Funda.DAL.Models;
using Funda.Domain.Enums;
using Funda.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Funda.DAL.Clients
{
    public interface IFundaPropertiesClient
    {
        Task<List<Makelaar>> GetPropertiesMakellars(string city, bool propertiesWithGarden, PropertyType propertyType, int page = 1, int pageSize = 1000);
    }
}
