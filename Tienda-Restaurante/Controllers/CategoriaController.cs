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

        public CategoriaController(ICategoriaRepository categoriaRepo)
        {
            _categoriaRepo = categoriaRepo;
        }

        public async Task<IActionResult> Categoria()
        {
            var categorias = await _categoriaRepo.GetCategoria();
            return View(categorias);
        }

        public IActionResult AddCategoria()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoria(CategoriaDTO categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            try
            {
                var categoriaToAdd = new Categoria { CategoriaName = categoria.CategoriaName, Id = categoria.Id };
                await _categoriaRepo.AddCategoria(categoriaToAdd);
                TempData["successMessage"] = "Categoría añadida exitosamente";
                return RedirectToAction(nameof(AddCategoria));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Categoria no ingresada";
                return View(categoria);
            }

        }

        public async Task<IActionResult> UpdateCategoria(int id)
        {
            var categoria = await _categoriaRepo.GetCategoriaById(id);
            if (categoria is null)
                throw new InvalidOperationException($"Categoria con id: {id} no encontrada");
            var categoriaToUpdate = new CategoriaDTO
            {
                Id = categoria.Id,
                CategoriaName = categoria.CategoriaName
            };
            return View(categoriaToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategoria(CategoriaDTO categoriaToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaToUpdate);
            }
            try
            {
                var categoria = new Categoria { CategoriaName = categoriaToUpdate.CategoriaName, Id = categoriaToUpdate.Id };
                await _categoriaRepo.UpdateCategoria(categoria);
                TempData["successMessage"] = "Categoria actualizada correctamente";
                return RedirectToAction(nameof(Categoria));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "La categoria no se pudo actualizar";
                return View(categoriaToUpdate);
            }

        }

        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var genre = await _categoriaRepo.GetCategoriaById(id);
            if (genre is null)
                throw new InvalidOperationException($"Categoria con id: {id} no encontrada");
            await _categoriaRepo.DeleteCategoria(genre);
            return RedirectToAction(nameof(Categoria));

        }
    }
}
