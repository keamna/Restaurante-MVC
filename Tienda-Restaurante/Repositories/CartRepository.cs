using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tienda_Restaurante.Areas.Identity.Data;
using Tienda_Restaurante.DTOs;
using Microsoft.AspNetCore.Identity.UI.Services;
using Tienda_Restaurante.Services;


namespace Tienda_Restaurante.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly IEmailSender _emailSender;
        private readonly ICuerpoCorreoService _cuerpoCorreo;

        public CartRepository(
            ApplicationDbContext db,
            IHttpContextAccessor httpcontextAccessor,
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            ICuerpoCorreoService cuerpoCorreo
        )
        {
            _db = db;
            _userManager = userManager;
            _httpcontextAccessor = httpcontextAccessor;
            _emailSender = emailSender;
            _cuerpoCorreo = cuerpoCorreo;
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
                    throw new UnauthorizedAccessException("El usuario no ha iniciado sesión");

                var carrito = await GetCart(usuarioId);
                if (carrito is null)
                    throw new InvalidOperationException("Carrito inválido");

                var carritoItem = _db.DetallesCarrito
                    .FirstOrDefault(a => a.CarritoId == carrito.Id && a.PlatilloId == platilloId);

                if (carritoItem is null)
                    throw new InvalidOperationException("No hay productos en el carrito");
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
                .ThenInclude(a => a.Stocks)
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

        public async Task<bool> DoCheckout(string correo)
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                var usuarioId = GetUserId();
                if (string.IsNullOrEmpty(usuarioId))
                    throw new UnauthorizedAccessException("El usuario no ha iniciado sesión");

                var cart = _db.Carritos.Where(x => x.UserId == usuarioId).FirstOrDefault();
                if (cart is null)
                    throw new InvalidOperationException("Carrito inválido");

                var carritoDetalle = _db.DetallesCarrito
                    .Include(c => c.Platillo)
                    .Where(a => a.CarritoId == cart.Id)
                    .ToList();

                if (carritoDetalle.Count == 0)
                    throw new InvalidOperationException("Carrito vacío");

                var pendingRecord = _db.OrdenesEstado.FirstOrDefault(s => s.EstadoNombre == "Pendiente");
                if (pendingRecord is null)
                    throw new InvalidOperationException("El estado de la orden no es pendiente");

                // Crear la orden
                var order = new Orden
                {
                    UserId = usuarioId,
                    Email = correo,
                    PaymentMethod = "Tarjeta",
                    IsPaid = true,
                    OrdenEstadoId = pendingRecord.Id,
                    FechaOrden = DateTime.UtcNow,
                    Address = "Restaurante",
                    MobileNumber = "22350522",
                    Name = correo,
                    IsDeleted  = false

                };
                _db.Ordenes.Add(order);
                _db.SaveChanges();

                var detallesCompra = carritoDetalle;

                // Crear los detalles de la orden
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

                    var stock = _db.Stocks.Where(a => a.PlatilloId == item.PlatilloId).FirstOrDefault();
                    if (stock == null)
                        throw new InvalidOperationException("El inventario es cero");

                    if (item.Cantidad > stock.Cantidad)
                        throw new InvalidOperationException($"Solo {stock.Cantidad} platillo(s) disponible(s) en el inventario");

                    stock.Cantidad -= item.Cantidad;
                }

                _db.SaveChanges();

                _db.DetallesCarrito.RemoveRange(carritoDetalle);
                _db.SaveChanges();

                transaction.Commit();

                string cuerpo = _cuerpoCorreo.GenerarCuerpoVenta(detallesCompra);
                await _emailSender.SendEmailAsync(correo, "Confirmación de Compra", cuerpo);

                return true;
            }
            catch (Exception ex)
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