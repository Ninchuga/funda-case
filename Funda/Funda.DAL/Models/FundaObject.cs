namespace Funda.DAL.Models
{
    internal class FundaObject
    {
        public string Adres { get; set; }
        public decimal Koopprijs { get; set; }
        public decimal KoopprijsTot { get; set; }
        public int MakelaarId { get; set; }
        public string MakelaarNaam { get; set; }
        public string Postcode { get; set; }
        public string Woonplaats { get; set; }
        public int AantalKamers { get; set; }
    }
}
