using Funda.Domain.Models;

namespace Funda.Application.Services
{
    public interface IMakelaarService
    {
        Task<List<Makelaar>> GetMakelaarsFor(string city, bool propertiesWithGarden);
    }
}
