using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Restaurante.Models
{
    [Table("Stock")]
    public class Stock
    {
        public int Id { get; set; }
        public int PlatilloId { get; set; }
        public int Cantidad { get; set; }
        public Platillo? Platillo { get; set; }
    }
}
