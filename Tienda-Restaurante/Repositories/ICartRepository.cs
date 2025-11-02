using Tienda_Restaurante.Models;

namespace Tienda_Restaurante.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int platilloId, int cantidad);
        Task<int> RemoveItem(int platilloId);
        Task<Carrito> GetUserCarrito();
        Task<int> GetCartItemCount(string usuarioId = "");
        Task<Carrito> GetCart(string usuarioId);
        Task<bool> DoCheckout();
    }
}
