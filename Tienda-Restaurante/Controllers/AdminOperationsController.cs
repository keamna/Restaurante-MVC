using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tienda_Restaurante.Constants;
using Tienda_Restaurante.DTOs;

namespace Tienda_Restaurante.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class AdminOperationsController : Controller
{
    private readonly IUserOrderRepository _userOrderRepository;
    private readonly ILogger<AdminOperationsController> _logger;

    public AdminOperationsController(IUserOrderRepository userOrderRepository, ILogger<AdminOperationsController> logger)
    {
        _userOrderRepository = userOrderRepository;
        _logger = logger;
    }

    public async Task<IActionResult> AllOrders()
    {
        _logger.LogInformation("Inicio del método AllOrders");
        var orders = await _userOrderRepository.UserOrders(true);
        _logger.LogInformation("Se obtuvieron {Count} órdenes del repositorio", orders?.Count() ?? 0);
        return View(orders);
    }

    public async Task<IActionResult> TogglePaymentStatus(int orderId)
    {
        _logger.LogInformation("Intentando cambiar el estado de pago de la orden con ID {OrderId}", orderId);
        try
        {
            await _userOrderRepository.TogglePaymentStatus(orderId);
            _logger.LogInformation("Se cambió el estado de pago correctamente para la orden con ID {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar el estado de pago para la orden con ID {OrderId}", orderId);
        }
        return RedirectToAction(nameof(AllOrders));
    }

    public async Task<IActionResult> UpdateOrderStatus(int orderId)
    {
        _logger.LogInformation("Cargando vista para actualizar estado de la orden con ID {OrderId}", orderId);
        var order = await _userOrderRepository.GetOrderById(orderId);
        if (order == null)
        {
            _logger.LogWarning("No se encontró la orden con ID {OrderId}", orderId);
            throw new InvalidOperationException($"Orden con el id:{orderId} no ha sido encontrado");
        }

        var orderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
        {
            return new SelectListItem
            {
                Value = orderStatus.Id.ToString(),
                Text = orderStatus.EstadoNombre,
                Selected = order.OrdenEstadoId == orderStatus.Id
            };
        });

        var data = new UpdateOrderStatusModel
        {
            OrderId = orderId,
            OrderStatusId = order.OrdenEstadoId,
            OrdenEstadoList = orderStatusList
        };

        _logger.LogInformation("Vista de actualización de estado preparada correctamente para la orden con ID {OrderId}", orderId);
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
    {
        _logger.LogInformation("Intentando actualizar el estado de la orden con ID {OrderId} a {OrderStatusId}", data.OrderId, data.OrderStatusId);
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo de actualización de estado no es válido para la orden con ID {OrderId}", data.OrderId);
                data.OrdenEstadoList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
                {
                    return new SelectListItem
                    {
                        Value = orderStatus.Id.ToString(),
                        Text = orderStatus.EstadoNombre,
                        Selected = orderStatus.Id == data.OrderStatusId
                    };
                });

                return View(data);
            }

            await _userOrderRepository.ChangeOrderStatus(data);
            _logger.LogInformation("El estado de la orden con ID {OrderId} fue actualizado correctamente", data.OrderId);
            TempData["msg"] = "Actualizado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado de la orden con ID {OrderId}", data.OrderId);
            TempData["msg"] = "Algo salió mal";
        }

        return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
    }

    public IActionResult Dashboard()
    {
        _logger.LogInformation("Acceso al Dashboard de administración");
        return View();
    }
}
