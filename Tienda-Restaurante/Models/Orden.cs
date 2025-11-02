using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Pkcs;

namespace Tienda_Restaurante.Models
{
    [Table("Orden")]
    public class Orden
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public DateTime FechaOrden { get; set; } = DateTime.UtcNow;

        [Required]
        public int OrdenEstadoId { get; set; }
        public bool IsDeleted { get; set; } = false;

        public OrdenEstado OrdenEstado { get; set; }

        public List<DetalleOrden> DetalleOrden { get; set; }
    }
}