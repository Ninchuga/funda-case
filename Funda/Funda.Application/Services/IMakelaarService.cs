using Funda.Domain.Enums;
using Funda.Domain.Models;

namespace Funda.Application.Services
{
    public interface IMakelaarService
    {
        Task<List<Makelaar>> GetMakelaarsFor(string city, bool propertiesWithGarden, PropertyType propertyType, int page = 1, int pageSize = 1000);
    }
}
