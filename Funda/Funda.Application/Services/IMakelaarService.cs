using Funda.Application.Models;

namespace Funda.Application.Services
{
    public interface IMakelaarService
    {
        Task<List<SellingMakelaarDto>> GetTopTenSellingMakelaarsFor(string city, bool propertiesWithGarden);
    }
}
