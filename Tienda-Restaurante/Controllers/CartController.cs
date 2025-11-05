using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tienda_Restaurante.DTOs;
using Tienda_Restaurante.Repositories;

namespace Tienda_Restaurante.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartRepository cartRepo, IHttpContextAccessor contextAccessor, UserManager<IdentityUser> userManager, ILogger<CartController> logger)
        {
            _cartRepo = cartRepo;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> AddItem(int platilloId, int cantidad = 1, int redirect = 0)
        {
            _logger.LogInformation("Agregando {Cantidad} unidad(es) del platillo con ID {PlatilloId} al carrito.", cantidad, platilloId);
            try
            {
                var cartCount = await _cartRepo.AddItem(platilloId, cantidad);
                _logger.LogInformation("Platillo agregado correctamente. Total de artículos en el carrito: {CartCount}", cartCount);

                if (redirect == 0)
                    return Ok(cartCount);

                return RedirectToAction("GetUserCart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar agregar el platillo con ID {PlatilloId} al carrito.", platilloId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        public async Task<IActionResult> RemoveItem(int platilloId)
        {
            _logger.LogInformation("Eliminando platillo con ID {PlatilloId} del carrito.", platilloId);
            try
            {
                var cartCount = await _cartRepo.RemoveItem(platilloId);
                _logger.LogInformation("Platillo eliminado. Total de artículos restantes: {CartCount}", cartCount);
                return RedirectToAction("GetUserCart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el platillo con ID {PlatilloId} del carrito.", platilloId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        public async Task<IActionResult> GetUserCart()
        {
            _logger.LogInformation("Obteniendo el carrito del usuario actual.");
            try
            {
                var cart = await _cartRepo.GetUserCart();
                if (cart == null)
                {
                    _logger.LogWarning("No se encontró un carrito activo para el usuario.");
                }
                else
                {
                    _logger.LogInformation("Carrito cargado correctamente con {CantidadItems} elementos.", cart.CarritoDetalles.Count);
                }
                return View(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el carrito del usuario.");
                return StatusCode(500, "Error al obtener el carrito");
            }
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            _logger.LogInformation("Obteniendo cantidad total de ítems en el carrito.");
            try
            {
                int cartItem = await _cartRepo.GetCartItemCount();
                _logger.LogInformation("El usuario tiene {CartItemCount} ítems en el carrito.", cartItem);
                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la cantidad total de ítems en el carrito.");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        public IActionResult Checkout()
        {
            _logger.LogInformation("Mostrando vista de Checkout.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            _logger.LogInformation("Iniciando proceso de Checkout.");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo de Checkout inválido. Se devolverá la vista con los errores.");
                return View(model);
            }

            try
            {
                var principal = _contextAccessor.HttpContext.User;
                string usuarioId = _userManager.GetUserName(principal);
                _logger.LogInformation("Usuario autenticado para Checkout: {UsuarioId}", usuarioId);

                bool isCheckedOut = await _cartRepo.DoCheckout(usuarioId);

                if (!isCheckedOut)
                {
                    _logger.LogWarning("El proceso de Checkout falló para el usuario {UsuarioId}", usuarioId);
                    return RedirectToAction(nameof(OrderFailure));
                }

                _logger.LogInformation("Checkout completado exitosamente para el usuario {UsuarioId}", usuarioId);
                return RedirectToAction(nameof(OrderSuccess));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el proceso de Checkout.");
                return RedirectToAction(nameof(OrderFailure));
            }
        }

        public IActionResult OrderSuccess()
        {
            _logger.LogInformation("Vista de orden completada mostrada.");
            var principal = _contextAccessor.HttpContext.User;
            string usuarioId = _userManager.GetUserName(principal);
            _cartRepo.DoCheckout(usuarioId);
            return View();
        }

        public IActionResult OrderFailure()
        {
            _logger.LogWarning("Vista de fallo en orden mostrada al usuario.");
            return View();
        }
    }
}
