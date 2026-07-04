using Funda.Domain.Enums;
using Funda.Domain.Models;

namespace Funda.DAL.Clients
{
    public interface IFundaObjectsClient
    {
        Task<(List<Makelaar> makelaars, int totalPages)> GetMakellarsDataFromObjects(string city, bool propertiesWithGarden, PropertyType propertyType, int page = 1, int pageSize = 25);
    }
}
