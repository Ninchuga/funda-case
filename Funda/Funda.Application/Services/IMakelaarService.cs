using Funda.Application.Models;
using Funda.Domain.Models;

namespace Funda.Application.Services
{
    public interface IMakelaarService
    {
        Task<Result<List<SellingMakelaarDto>>> GetTopTenSellingMakelaarsFor(string city, bool propertiesWithGarden, CancellationToken cancellationToken);
    }
}
