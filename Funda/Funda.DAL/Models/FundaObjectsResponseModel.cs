using System.Text.Json.Serialization;

namespace Funda.DAL.Models
{
    internal class FundaObjectsResponseModel
    {
        [JsonPropertyName("Metadata")]
        public PropertiesMetadata Metadata { get; set; }

        [JsonPropertyName("Objects")]
        public IEnumerable<FundaObject> Objects { get; set; } = [];

        [JsonPropertyName("Paging")]
        public Paging Paging { get; set; }

        [JsonPropertyName("TotaalAantalObjecten")]
        public int TotalNumberOfObjects { get; set; }
    }

    internal class PropertiesMetadata
    {
        [JsonPropertyName("Titel")]
        public string Title { get; set; }

        [JsonPropertyName("ObjectType")]
        public string ObjectType { get; set; }

        [JsonPropertyName("Omschrijving")]
        public string Description { get; set; }
    }

    internal class Paging
    {
        [JsonPropertyName("AantalPaginas")]
        public int TotalPages { get; set; }

        [JsonPropertyName("HuidigePagina")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("VolgendeUrl")]
        public string NextUrl { get; set; }

        [JsonPropertyName("VorigeUrl")]
        public string PreviousUrl { get; set; }
    }
}
