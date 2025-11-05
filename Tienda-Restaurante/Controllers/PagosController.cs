using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Tienda_Restaurante.Repositories;
using Tienda_Restaurante.Areas.Identity.Data;

namespace Tienda_Restaurante.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly ICartRepository _cartRepo;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<PagosController> _logger;

        public PagosController(
            ICartRepository cartRepo,
            ApplicationDbContext db,
            IConfiguration config,
            ILogger<PagosController> logger)
        {
            _cartRepo = cartRepo;
            _db = db;
            _config = config;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSesionCheckout()
        {
            try
            {
                _logger.LogInformation("Iniciando creación de sesión de pago");

                var cart = await _cartRepo.GetUserCart();
                if (cart == null || cart.CarritoDetalles == null || !cart.CarritoDetalles.Any())
                {
                    _logger.LogWarning("Carrito vacío o nulo al intentar crear sesión de pago");
                    return BadRequest("Carrito vacío");
                }

                var lineItems = cart.CarritoDetalles.Select(d => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "crc",
                        UnitAmount = (long)(d.PrecioUnitario * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = d.Platillo.PlatilloName
                        }
                    },
                    Quantity = d.Cantidad
                }).ToList();

                var domain = $"{Request.Scheme}://{Request.Host}";
                var options = new SessionCreateOptions
                {
                    Mode = "payment",
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    SuccessUrl = $"{domain}/Cart/OrderSuccess",
                    CancelUrl = $"{domain}/Cart/OrderFailure"
                };

                var service = new SessionService();
                var session = service.Create(options);

                _logger.LogInformation("Sesión de Stripe creada correctamente. Id: {SessionId}", session.Id);

                return Json(new { id = session.Id, publishableKey = _config["Stripe:PublishableKey"] });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al procesar el pago: {Mensaje}", ex.Message);
                return StatusCode(500, "Error al procesar el pago");
            }
        }
    }
}
