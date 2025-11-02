using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tienda_Restaurante.Areas.Identity.Data;

namespace Tienda_Restaurante.Repositories
{
    
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public UserOrderRepository(ApplicationDbContext db, 
            IHttpContextAccessor httpcontextAccessor, 
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _httpcontextAccessor = httpcontextAccessor;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Orden>> UserOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("El usuario no ha iniciado sesión");
            var orders = await _db.Ordenes
                .Include(x => x.OrdenEstado)
                .Include(x=>x.DetalleOrden)
                .ThenInclude(x=>x.Platillo)
                .ThenInclude(x => x.Categoria)
                .Where(a=>a.UserId==userId)
                .ToListAsync();

            return orders;
        }

        private string GetUserId()
        {
            var principal = _httpcontextAccessor.HttpContext.User;
            string usuarioId = _userManager.GetUserId(principal);
            return usuarioId;
        }
    }
}
