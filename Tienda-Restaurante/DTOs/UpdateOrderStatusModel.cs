using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tienda_Restaurante.DTOs;

public class UpdateOrderStatusModel
{
    public int OrderId { get; set; }

    [Required]
    public int OrderStatusId { get; set; }

    public IEnumerable<SelectListItem>? OrdenEstadoList { get; set; }
}
