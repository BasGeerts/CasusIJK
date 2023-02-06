using System.ComponentModel.DataAnnotations;

namespace Casus.Objects
{
    public class Product
    {
        [Key]
        public string ProductId { get; set; }
        public decimal Prijs { get; set; }

        public Product(string productId, decimal prijs)
        {
            ProductId = productId;
            Prijs = prijs;
        }
    }
}
