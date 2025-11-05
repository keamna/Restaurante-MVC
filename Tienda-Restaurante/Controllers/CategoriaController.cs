using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda_Restaurante.Constants;
using Tienda_Restaurante.DTOs;

namespace Tienda_Restaurante.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(ICategoriaRepository categoriaRepo, ILogger<CategoriaController> logger)
        {
            _categoriaRepo = categoriaRepo;
            _logger = logger;
        }

        public async Task<IActionResult> Categoria()
        {
            _logger.LogInformation("Obteniendo lista de categorías...");
            try
            {
                var categorias = await _categoriaRepo.GetCategoria();
                _logger.LogInformation("Se obtuvieron {Cantidad} categorías.", categorias.Count());
                return View(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías.");
                TempData["errorMessage"] = "Error al cargar las categorías.";
                return View(Enumerable.Empty<CategoriaDTO>());
            }
        }

        public IActionResult AddCategoria()
        {
            _logger.LogInformation("Mostrando vista para agregar nueva categoría.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoria(CategoriaDTO categoria)
        {
            _logger.LogInformation("Intentando agregar nueva categoría: {CategoriaName}", categoria.CategoriaName);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al intentar agregar categoría: {CategoriaName}", categoria.CategoriaName);
                return View(categoria);
            }

            try
            {
                var categoriaToAdd = new Categoria { CategoriaName = categoria.CategoriaName, Id = categoria.Id };
                await _categoriaRepo.AddCategoria(categoriaToAdd);
                _logger.LogInformation("Categoría '{CategoriaName}' añadida exitosamente.", categoria.CategoriaName);
                TempData["successMessage"] = "Categoría añadida exitosamente";
                return RedirectToAction(nameof(AddCategoria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar la categoría: {CategoriaName}", categoria.CategoriaName);
                TempData["errorMessage"] = "Categoría no ingresada";
                return View(categoria);
            }
        }

        public async Task<IActionResult> UpdateCategoria(int id)
        {
            _logger.LogInformation("Obteniendo categoría con ID {Id} para actualización.", id);
            try
            {
                var categoria = await _categoriaRepo.GetCategoriaById(id);
                if (categoria is null)
                {
                    _logger.LogWarning("Categoría con ID {Id} no encontrada.", id);
                    throw new InvalidOperationException($"Categoría con id: {id} no encontrada");
                }

                var categoriaToUpdate = new CategoriaDTO
                {
                    Id = categoria.Id,
                    CategoriaName = categoria.CategoriaName
                };
                _logger.LogInformation("Categoría {CategoriaName} preparada para edición.", categoria.CategoriaName);
                return View(categoriaToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la categoría con ID {Id}.", id);
                TempData["errorMessage"] = "Error al cargar la categoría.";
                return RedirectToAction(nameof(Categoria));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategoria(CategoriaDTO categoriaToUpdate)
        {
            _logger.LogInformation("Intentando actualizar categoría con ID {Id}.", categoriaToUpdate.Id);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al intentar actualizar categoría con ID {Id}.", categoriaToUpdate.Id);
                return View(categoriaToUpdate);
            }

            try
            {
                var categoria = new Categoria { CategoriaName = categoriaToUpdate.CategoriaName, Id = categoriaToUpdate.Id };
                await _categoriaRepo.UpdateCategoria(categoria);
                _logger.LogInformation("Categoría {CategoriaName} actualizada correctamente.", categoriaToUpdate.CategoriaName);
                TempData["successMessage"] = "Categoría actualizada correctamente";
                return RedirectToAction(nameof(Categoria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría con ID {Id}.", categoriaToUpdate.Id);
                TempData["errorMessage"] = "La categoría no se pudo actualizar";
                return View(categoriaToUpdate);
            }
        }

        public async Task<IActionResult> DeleteCategoria(int id)
        {
            _logger.LogInformation("Intentando eliminar categoría con ID {Id}.", id);
            try
            {
                var categoria = await _categoriaRepo.GetCategoriaById(id);
                if (categoria is null)
                {
                    _logger.LogWarning("No se encontró la categoría con ID {Id} para eliminar.", id);
                    throw new InvalidOperationException($"Categoría con id: {id} no encontrada");
                }

                await _categoriaRepo.DeleteCategoria(categoria);
                _logger.LogInformation("Categoría {CategoriaName} eliminada correctamente.", categoria.CategoriaName);
                return RedirectToAction(nameof(Categoria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría con ID {Id}.", id);
                TempData["errorMessage"] = "No se pudo eliminar la categoría.";
                return RedirectToAction(nameof(Categoria));
            }
        }
    }
}
