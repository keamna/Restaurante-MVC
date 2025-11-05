using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tienda_Restaurante.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepository;
        private readonly ILogger<UserOrderController> _logger;

        public UserOrderController(IUserOrderRepository userOrderRepository, ILogger<UserOrderController> logger)
        {
            _userOrderRepository = userOrderRepository;
            _logger = logger;
        }

        public async Task<IActionResult> UserOrders()
        {
            _logger.LogInformation("Accediendo a la vista de órdenes del usuario.");

            var orders = await _userOrderRepository.UserOrders();
            _logger.LogInformation("Órdenes obtenidas correctamente.");

            return View(orders);
        }
    }

}
