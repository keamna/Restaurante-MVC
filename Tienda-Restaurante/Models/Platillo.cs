using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Restaurante.Models
{
    [Table("Platillo")]
    public class Platillo
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? PlatilloName { get; set; }

        [Required]
        public double Precio { get; set; }

        public string? ImagenUrl { get; set; }

        [MaxLength(120)]
        public string Descripcion { get; set; }

        [Required]
        // Un platillo pertenece a una sola categoria
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public List<DetalleOrden> DetalleOrden { get; set; }
        public List<DetalleCarrito> DetalleCarrito { get; set; }


        [NotMapped]
        public string CategoriaNombre { get; set; }

    }
}