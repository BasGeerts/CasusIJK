using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casus.Objects
{
    [PrimaryKey(nameof(Ordernr), nameof(ProductId), nameof(Aantal))]
    public class Orderregel
    {
        public int Ordernr { get; set; }
        public string ProductId { get; set; }
        public int Aantal { get; set; }

        public Orderregel(int ordernr, string productId, int aantal)
        {
            Ordernr = ordernr;
            ProductId = productId;
            Aantal = aantal;
        }
    }
}
