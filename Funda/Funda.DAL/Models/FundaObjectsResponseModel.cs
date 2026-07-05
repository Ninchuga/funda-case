using System.Text.Json.Serialization;

namespace Funda.DAL.Models
{
    internal class FundaObjectsResponseModel
    {
        public PropertiesMetadata Metadata { get; set; }
        public IEnumerable<FundaObject> Objects { get; set; } = [];
        public Paging Paging { get; set; }
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
