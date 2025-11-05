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

        public PagosController(ICartRepository cartRepo, ApplicationDbContext db, IConfiguration config)
        {
            _cartRepo = cartRepo;
            _db = db;
            _config = config;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSesionCheckout()
        {
            var cart = await _cartRepo.GetUserCart();
            if (cart == null || cart.CarritoDetalles == null || !cart.CarritoDetalles.Any())
                return BadRequest("Carrito vacío");

            
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

            return Json(new { id = session.Id, publishableKey = _config["Stripe:PublishableKey"] });
        }
    }
}
