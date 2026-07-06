using Funda.Domain.Enums;

namespace Funda.DAL.Models
{
    public record GetMakelaarsDataFromObjectsRequest(
        string City,
        bool PropertiesWithGarden,
        PropertyType PropertyType,
        int PageNumber,
        int PageSize
    );
}
