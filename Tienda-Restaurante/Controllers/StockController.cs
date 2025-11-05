using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tienda_Restaurante.Constants;
using Tienda_Restaurante.DTOs;

namespace Tienda_Restaurante.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<StockController> _logger;

        public StockController(IStockRepository stockRepository, ILogger<StockController> logger)
        {
            _stockRepository = stockRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Stock(string sterm = "")
        {
            _logger.LogInformation("Accediendo a la vista de stock. Filtro: {Filtro}", sterm);
            var stocks = await _stockRepository.GetStocks(sterm);
            _logger.LogInformation($"Stock obtenido correctamente.");
            return View(stocks);
        }

        public async Task<IActionResult> ManangeStock(int platilloId)
        {
            _logger.LogInformation("Accediendo a la gestión de stock para PlatilloId {PlatilloId}", platilloId);
            var existingStock = await _stockRepository.GetStockByPlatilloId(platilloId);
            var stock = new StockDTO
            {
                PlatilloId = platilloId,
                Cantidad = existingStock != null
                    ? existingStock.Cantidad
                    : 0
            };
            _logger.LogInformation("Stock cargado para PlatilloId {PlatilloId} con cantidad {Cantidad}", platilloId, stock.Cantidad);
            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> ManangeStock(StockDTO stock)
        {
            _logger.LogInformation("Intento de actualización de stock para PlatilloId {PlatilloId} con cantidad {Cantidad}", stock.PlatilloId, stock.Cantidad);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al actualizar stock para PlatilloId {PlatilloId}", stock.PlatilloId);
                return View(stock);
            }

            try
            {
                await _stockRepository.ManageStock(stock);
                _logger.LogInformation("Stock actualizado correctamente para PlatilloId {PlatilloId}", stock.PlatilloId);
                TempData["successMessage"] = "Inventario actualizado correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar stock para PlatilloId {PlatilloId}", stock.PlatilloId);
                TempData["errorMessage"] = "Algo ha salido mal";
            }

            return RedirectToAction(nameof(Stock));
        }
    }
}
