using Funda.Domain.Models;

namespace Funda.DAL.Models
{
    public record GetMakelaarsDataFromObjectsResponse(
        IReadOnlyList<Makelaar> Makelaars,
        int TotalPages,
        int TotalObjects);
}
