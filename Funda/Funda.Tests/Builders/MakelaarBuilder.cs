using Funda.Domain.Models;

namespace Funda.Tests.Builders
{
    public class MakelaarBuilder
    {
        private int Id = 1;
        private string Name = "Test Makelaar";
        private int NumberOfPropertiesForSale = 1;

        public MakelaarBuilder WithId(int id)
        {
            Id = id;
            return this;
        }

        public MakelaarBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public Makelaar Build()
        {
            return new Makelaar(
                Id,
                Name,
                NumberOfPropertiesForSale
            );
        }
    }
}
