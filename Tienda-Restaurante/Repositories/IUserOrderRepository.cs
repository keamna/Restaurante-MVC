using Tienda_Restaurante.DTOs;
using Tienda_Restaurante.Models;

namespace Tienda_Restaurante.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Orden>> UserOrders(bool getAll = false);
        Task ChangeOrderStatus(UpdateOrderStatusModel data);
        Task TogglePaymentStatus(int orderId);
        Task<Orden?> GetOrderById(int id);
        Task<IEnumerable<OrdenEstado>> GetOrderStatuses();
    }
}