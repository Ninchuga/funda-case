using System.Text.Json.Serialization;

namespace Funda.DAL.Models
{
    internal class FundaObject
    {
        [JsonPropertyName("Adres")]
        public string Address { get; set; }

        [JsonPropertyName("Koopprijs")]
        public decimal? Price { get; set; }

        [JsonPropertyName("KoopprijsTot")]
        public decimal? PurchasePrice { get; set; }

        [JsonPropertyName("MakelaarId")]
        public int MakelaarId { get; set; }

        [JsonPropertyName("MakelaarNaam")]
        public string MakelaarName { get; set; }

        [JsonPropertyName("Postcode")]
        public string Postcode { get; set; }

        [JsonPropertyName("Woonplaats")]
        public string Place { get; set; }

        [JsonPropertyName("AantalKamers")]
        public int? NumberOfRooms { get; set; }

        [JsonPropertyName("BronCode")]
        public string SourceCode { get; set; }

        [JsonPropertyName("ChildrenObjects")]
        public List<ChildrenObject> ChildrenObjects { get; set; } = [];
    }

    internal class ChildrenObject : FundaObject
    {
    }
}
