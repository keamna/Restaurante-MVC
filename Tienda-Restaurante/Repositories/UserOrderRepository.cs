using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tienda_Restaurante.Areas.Identity.Data;
using Tienda_Restaurante.DTOs;

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

        public async Task ChangeOrderStatus(UpdateOrderStatusModel data)
        {
            var order = await _db.Ordenes.FindAsync(data.OrderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Orden con id:{data.OrderId} no existe");
            }
            order.OrdenEstadoId = data.OrderStatusId;
            await _db.SaveChangesAsync();
        }

        public Task<Orden?> GetOrderById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrdenEstado>> GetOrderStatuses()
        {
            return await _db.OrdenesEstado.ToListAsync();
        }

        public async Task TogglePaymentStatus(int orderId)
        {
            var order = await _db.Ordenes.FindAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order con id:{orderId} no existe");
            }
            order.IsPaid = !order.IsPaid;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Orden>> UserOrders(bool getAll = false)
        {
            var orders = _db.Ordenes
                            .Include(x => x.OrdenEstado)
                            .Include(x => x.DetalleOrden)
                            .ThenInclude(x => x.Platillo)
                            .ThenInclude(x => x.Categoria).AsQueryable();
            if (!getAll)
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("El usuario no ha iniciado sesión");
                orders = orders.Where(a => a.UserId == userId);
                return await orders.ToListAsync();
            }
            return await orders.ToListAsync();
        }

        private string GetUserId()
        {
            var principal = _httpcontextAccessor.HttpContext.User;
            string usuarioId = _userManager.GetUserId(principal);
            return usuarioId;
        }
    }
}
