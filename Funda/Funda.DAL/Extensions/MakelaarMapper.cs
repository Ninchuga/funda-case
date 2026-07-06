using Funda.Domain.Models;

namespace Funda.DAL.Extensions
{
    internal static class MakelaarMapper
    {
        public static List<Makelaar> ToMakelaars(this Models.FundaObjectsResponseModel objectsResponseModel)
        {
            if (objectsResponseModel is null)
                return [];

            var makelaars = objectsResponseModel.Objects?.Select(x => new { x.MakelaarId, x.MakelaarName }).ToList() ?? [];
            var makelaarsFromChildrenObjects = objectsResponseModel.Objects?.SelectMany(x => x.ChildrenObjects ?? []).Select(x => new { x.MakelaarId, x.MakelaarName }).ToList() ?? [];
            makelaars.AddRange(makelaarsFromChildrenObjects);

            return [.. makelaars.Select(mak => new Makelaar(mak.MakelaarId, mak.MakelaarName, NumberOfPropertiesForSale: 0))];
        }
    }
}
