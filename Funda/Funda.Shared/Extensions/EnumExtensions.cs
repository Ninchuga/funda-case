using Funda.Domain.Enums;

namespace Funda.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static string ToLowerString(this PropertyType propertyType) => propertyType.ToString().ToLower();
    }
}
