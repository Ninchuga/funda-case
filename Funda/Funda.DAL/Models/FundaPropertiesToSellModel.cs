namespace Funda.DAL.Models
{
    internal class FundaPropertiesToSellModel
    {
        public PropertiesMetadata Metadata { get; set; }
        public IEnumerable<FundaObject> Objects { get; set; }
    }

    internal class PropertiesMetadata
    {
        public string Titel { get; set; }
        public string ObjectType { get; set; }
        public string Omschrijving { get; set; }
    }
}
