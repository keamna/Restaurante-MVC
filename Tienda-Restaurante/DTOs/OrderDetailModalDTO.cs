namespace Tienda_Restaurante.DTOs;

public class OrderDetailModalDTO
{
    public string DivId { get; set; }
    public IEnumerable<DetalleOrden> DetalleOrden { get; set; }
}
