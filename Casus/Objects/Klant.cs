using Casus;
namespace Casus.Objects
{
    public class Klant
    {
        public int Id { get; set; }
        public string Naam { get; set; }

        public Klant(int id, string naam)
        {
            Id = id;
            Naam = naam;
        }
    }
}