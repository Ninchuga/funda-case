using Funda.Domain.Models;

namespace Funda.Application.Services
{
    public interface IMakelaarService
    {
        Task<List<Makelaar>> GetTopTenMakelaarsFor(string city, bool propertiesWithGarden);
    }
}
