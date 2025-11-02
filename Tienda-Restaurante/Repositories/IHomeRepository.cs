using Tienda_Restaurante.Models;

namespace Tienda_Restaurante.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Platillo>> GetPlatillos(string sTerm = "", int categoriaId = 0);
        Task<IEnumerable<Categoria>> Categorias();
    }
}
