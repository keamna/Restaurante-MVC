using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        [MaxLength(30)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? MobileNumber { get; set; }

        [Required]
        [MaxLength(200)]

        public string? Address { get; set; }

        [Required]
        [MaxLength(30)]
        public string? PaymentMethod { get; set; }

        public OrdenEstado OrdenEstado { get; set; }

        public List<DetalleOrden> DetalleOrden { get; set; }
    }
}