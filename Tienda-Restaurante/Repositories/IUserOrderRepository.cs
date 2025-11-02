namespace Tienda_Restaurante.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Orden>> UserOrders();
    }
}