using Funda.Domain.Enums;
using Funda.Domain.Models;

namespace Funda.DAL.Clients
{
    public interface IFundaPropertiesClient
    {
        Task<(List<Makelaar> makelaars, int totalPages)> GetPropertiesMakellars(string city, bool propertiesWithGarden, PropertyType propertyType, int page = 1, int pageSize = 100);
    }
}
