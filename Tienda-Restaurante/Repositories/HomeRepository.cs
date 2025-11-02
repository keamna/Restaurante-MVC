using Tienda_Restaurante1.Models;

namespace Tienda_Restaurante.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly TiendaDbContext _db;
        public HomeRepository(TiendaDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Categoria>> Categorias()
        {
            return await _db.Categorias.ToListAsync();
        }
        public async Task<IEnumerable<Platillo>> GetPlatillos(string sTerm = "", int categoriaId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Platillo> platillos = await (from platillo in _db.Platillos
                                                     join Categoria in _db.Categorias
                                                     on platillo.CategoriaId equals Categoria.Id
                                                     where string.IsNullOrWhiteSpace(sTerm) || (platillo != null && platillo.PlatilloName.ToLower().StartsWith(sTerm))
                                                     select new Platillo
                                                     {
                                                         Id = platillo.Id,
                                                         ImagenUrl = platillo.ImagenUrl,
                                                         PlatilloName = platillo.PlatilloName,
                                                         Descripcion = platillo.Descripcion,
                                                         Precio = platillo.Precio,
                                                         CategoriaId = platillo.CategoriaId,
                                                         CategoriaNombre = Categoria.CategoriaName
                                                     }
                             ).ToListAsync();
            if (categoriaId > 0)
            {
                platillos = platillos.Where(a => a.CategoriaId == categoriaId).ToList();
            }
            return platillos;
        }
    }
}
