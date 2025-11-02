using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tienda_Restaurante.Areas.Identity.Data;

namespace Tienda_Restaurante.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public CartRepository(
            ApplicationDbContext db,
            IHttpContextAccessor httpcontextAccessor,
            UserManager<IdentityUser> userManager
        )
        {
            _db = db;
            _userManager = userManager;
            _httpcontextAccessor = httpcontextAccessor;
        }

        public async Task<int> AddItem(int platilloId, int cantidad)
        {
            string usuarioId = GetUserId();
            using var transaccion = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(usuarioId))
                    throw new UnauthorizedAccessException("El usuario no ha iniciado sesión");

                var carrito = await GetCart(usuarioId);
                if (carrito is null)
                {
                    carrito = new Carrito()
                    {
                        UserId = usuarioId
                    };
                    _db.Carritos.Add(carrito);
                }

                _db.SaveChanges();

                var carritoItem = _db.DetallesCarrito
                    .FirstOrDefault(a => a.CarritoId == carrito.Id && a.PlatilloId == platilloId);

                if (carritoItem is not null)
                {
                    carritoItem.Cantidad += cantidad;
                }
                else
                {
                    var platillo = _db.Platillos.Find(platilloId);
                    carritoItem = new DetalleCarrito
                    {
                        PlatilloId = platilloId,
                        CarritoId = carrito.Id,
                        Cantidad = cantidad,
                        PrecioUnitario = platillo.Precio
                    };
                    _db.DetallesCarrito.Add(carritoItem);
                }

                _db.SaveChanges();
                transaccion.Commit();
            }
            catch (Exception)
            {
            }

            var cartItemCount = await GetCartItemCount(usuarioId);
            return cartItemCount;
        }
        
        public async Task<int> RemoveItem(int platilloId)
        {
            string usuarioId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(usuarioId))
                    throw new Exception("El usuario no ha iniciado sesión");

                var carrito = await GetCart(usuarioId);
                if (carrito is null)
                    throw new Exception("Carrito inválido");

                var carritoItem = _db.DetallesCarrito
                    .FirstOrDefault(a => a.CarritoId == carrito.Id && a.PlatilloId == platilloId);

                if (carritoItem is null)
                    throw new Exception("No hay productos en el carrito");
                else if (carritoItem.Cantidad == 1)
                    _db.DetallesCarrito.Remove(carritoItem);
                else
                    carritoItem.Cantidad -= 1;

                _db.SaveChanges();
            }
            catch (Exception)
            {
            }

            var cartItemCount = await GetCartItemCount(usuarioId);
            return cartItemCount;
        }

        public async Task<Carrito> GetUserCart()
        {
            var usuarioId = GetUserId();
            if (usuarioId == null)
                throw new InvalidOperationException("UsuarioId Inválido");

            var carrito = await _db.Carritos
                .Include(a => a.CarritoDetalles)
                .ThenInclude(a => a.Platillo)
                .ThenInclude(a => a.Categoria)
                .Where(a => a.UserId == usuarioId).FirstOrDefaultAsync();

            return carrito;
        }

        public async Task<Carrito> GetCart(string usuarioId)
        {
            var cart = await _db.Carritos.FirstOrDefaultAsync(x => x.UserId == usuarioId);
            return cart;
        }

        public async Task<int> GetCartItemCount(string usuarioId = "")
        {
            if (string.IsNullOrEmpty(usuarioId)) 
            {
                usuarioId = GetUserId();
            }

            var data = await (from cart in _db.Carritos
                              join detalleCarrito in _db.DetallesCarrito
                              on cart.Id equals detalleCarrito.CarritoId
                              where cart.UserId == usuarioId
                              select new { detalleCarrito }
                              ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckout()
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                var usuarioId = GetUserId();
                if (string.IsNullOrEmpty(usuarioId))
                    throw new Exception("El usuario no ha iniciado sesión");
                var cart = await GetCart(usuarioId);
                if (cart is null)
                    throw new Exception("Carrito inválido");
                var carritoDetalle = _db.DetallesCarrito
                    .Where(a => a.CarritoId == cart.Id).ToList();
                if (carritoDetalle.Count == 0)
                    throw new Exception("Carrito vacío");
                var order = new Orden
                {
                    UserId = usuarioId,
                    FechaOrden = DateTime.UtcNow,
                    OrdenEstadoId = 1, // Pendiente
                };
                _db.Ordenes.Add(order);
                _db.SaveChanges();
                foreach (var item in carritoDetalle)
                {
                    var ordenDetalle = new DetalleOrden
                    {
                        PlatilloId = item.PlatilloId,
                        OrdenId = order.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario
                    };
                    _db.DetalleOrdenes.Add(ordenDetalle);
                }
                _db.SaveChanges();

                _db.DetallesCarrito.RemoveRange(carritoDetalle);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GetUserId()
        {
            var principal = _httpcontextAccessor.HttpContext.User;
            string usuarioId = _userManager.GetUserId(principal);
            return usuarioId;
        }



    }
}