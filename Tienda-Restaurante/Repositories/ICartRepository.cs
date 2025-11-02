namespace Tienda_Restaurante.Repositories
{
    public interface ICartRepository
    {
        Task<int> AddItem(int bookId, int qty);
        Task<int> RemoveItem(int bookId);
        Task<Carrito> GetUserCart();
        Task<int> GetCartItemCount(string userId = "");
        Task<Carrito> GetCart(string userId);
        Task<bool> DoCheckout();
    }
}
