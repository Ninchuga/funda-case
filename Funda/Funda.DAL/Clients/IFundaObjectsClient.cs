using Funda.DAL.Models;
using Funda.Domain.Models;

namespace Funda.DAL.Clients
{
    public interface IFundaObjectsClient
    {
        Task<Result<GetMakelaarsDataFromObjectsResponse>> GetMakellarsDataFromObjects(GetMakelaarsDataFromObjectsRequest request, CancellationToken cancellationToken);
    }
}
