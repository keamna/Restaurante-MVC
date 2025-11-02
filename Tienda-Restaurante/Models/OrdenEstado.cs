using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Restaurante.Models
{
    [Table("EstadoOrden")]
    public class OrdenEstado
    {
        public int Id { get; set; }

        [Required]
        public int EstadoId { get; set; }

        [Required, MaxLength(20)]
        public string? EstadoNombre { get; set; }

    }
}
