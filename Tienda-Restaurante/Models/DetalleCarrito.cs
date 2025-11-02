using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Restaurante.Models
{
    [Table("DetalleCarrito")]
    public class DetalleCarrito
    {
        public int Id { get; set; }

        [Required]
        public int CarritoId { get; set; }

        [Required]
        public int PlatilloId { get; set; }
        public Platillo Platillo { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public double PrecioUnitario { get; set; }
        public Carrito Carrito { get; set; }
    }
}
