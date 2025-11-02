using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Restaurante.Models
{
    [Table("Carrito")]
    public class Carrito
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<DetalleCarrito> CarritoDetalles { get; set; }
    }
}