namespace Tienda_Restaurante.DTOs
{
    public class PlatilloDisplayModel
    {
        public IEnumerable<Platillo> Platillos { get; set; }
        public IEnumerable<Categoria> Categorias { get; set; }

        public string STerm { get; set; } = "";
        public int CategoriaId { get; set; } = 0;



    }
}
