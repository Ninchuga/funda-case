using Funda.Domain.Enums;

namespace Funda.DAL.Models
{
    internal record GetMakelaarsDataFromObjectsRequest(
        string City,
        bool PropertiesWithGarden,
        PropertyType PropertyType,
        int PageNumber,
        int PageSize
    );
}
