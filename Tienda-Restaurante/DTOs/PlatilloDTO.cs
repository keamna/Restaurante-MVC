using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tienda_Restaurante.DTOs
{
    public class PlatilloDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string? PlatilloName { get; set; }

        [Required]
        public double Precio { get; set; }
        public string? Descripcion { get; set; }

        public string? ImageURL { get; set; }
        [Required]
        public int CategoriaId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IEnumerable<SelectListItem>? CategoriaList { get; set; }
    }
}
