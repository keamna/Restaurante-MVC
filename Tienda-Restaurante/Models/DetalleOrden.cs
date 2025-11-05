using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Restaurante.Models
{
    [Table("DetalleOrden")]
    public class DetalleOrden
    {
        public int Id { get; set; }

        [Required]
        public int OrdenId { get; set; }

        public int PlatilloId { get; set; }
        public Platillo Platillo { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public Orden Orden { get; set; }


    }
}