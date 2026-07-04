using Funda.Application.Models;
using Funda.Domain.Models;

namespace Funda.Application.Extensions
{
    internal static class MakelaarMapper
    {
        public static SellingMakelaarDto ToDto(this Makelaar makelaar)
        {
            return makelaar is null
                ? new SellingMakelaarDto(Name: string.Empty, NumberOfPropertiesForSale: 0)
                : new SellingMakelaarDto(makelaar.Name, makelaar.NumberOfPropertiesForSale);
        }
    }
}
