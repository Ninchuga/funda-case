using Funda.Domain.Models;

namespace Funda.DAL.Extensions
{
    internal static class MakelaarMapper
    {
        public static List<Makelaar> ToMakelaars(this Models.FundaPropertiesToSellModel propertiesModel)
        {
            if (propertiesModel is null)
                return [];

            var makelaars = propertiesModel.Objects?.Select(x => new { x.MakelaarId, x.MakelaarNaam }).ToList() ?? [];
            var makelaarsFromChildrenObjects = propertiesModel.Objects?.SelectMany(x => x.ChildrenObjects ?? []).Select(x => new { x.MakelaarId, x.MakelaarNaam }).ToList() ?? [];
            makelaars.AddRange(makelaarsFromChildrenObjects);

            return [.. makelaars.Select(mak => new Makelaar(mak.MakelaarId, mak.MakelaarNaam))];
        }
    }
}
