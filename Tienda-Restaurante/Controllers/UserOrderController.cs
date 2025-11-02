using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tienda_Restaurante.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepository;
        public UserOrderController(IUserOrderRepository userOrderRepository) 
        {
            _userOrderRepository = userOrderRepository;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _userOrderRepository.UserOrders();
            return View(orders);
        }
    }
}
