using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda_Restaurante.Constants;
using Tienda_Restaurante.DTOs;

namespace Tienda_Restaurante.Controllers
{
    [Authorize(Roles =nameof(Roles.Admin))]
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepository;

        public StockController(IStockRepository stockRepository) 
        {
            _stockRepository = stockRepository;
        }

        // aqui
        public async Task<IActionResult> Stock(string sterm = "")
        {
            var stocks = await _stockRepository.GetStocks(sterm);
            return View(stocks);
        }

        public async Task<IActionResult> ManangeStock(int platilloId)
        {
            var existingStock = await _stockRepository.GetStockByPlatilloId(platilloId);
            var stock = new StockDTO
            {
                PlatilloId = platilloId,
                Cantidad = existingStock != null
            ? existingStock.Cantidad : 0
            };
            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> ManangeStock(StockDTO stock)
        {
            if (!ModelState.IsValid)
                return View(stock);
            try
            {
                await _stockRepository.ManageStock(stock);
                TempData["successMessage"] = "Inventario actualizado correctamente";
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Algo ha salido mal";
            }

            return RedirectToAction(nameof(Stock));
        }
    }
}
