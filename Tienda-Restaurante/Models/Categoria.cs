using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_Restaurante.Models
{
    [Table("Categoria")]
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string CategoriaName { get; set; }

        // Una catagoria puede tener muchos platillos
        public List<Platillo> Platillos { get; set; }

    }
}