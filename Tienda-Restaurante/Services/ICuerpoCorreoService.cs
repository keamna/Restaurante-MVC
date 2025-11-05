using System.Text;

namespace Tienda_Restaurante.Services
{
    public interface ICuerpoCorreoService
    {
        public string GenerarCuerpoVenta(List<DetalleCarrito> detalles);
        public string GenerarCuerpoReserva(List<Reserva> reservas);


    }
}