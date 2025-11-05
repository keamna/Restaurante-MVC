using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tienda_Restaurante.DTOs;
using Tienda_Restaurante.Models;
using Tienda_Restaurante.Views.Shared;

namespace Tienda_Restaurante.Controllers
{
    public class PlatilloController : Controller
    {
        private readonly IPlatilloRepository _platilloRepo;
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly IFileService _fileService;
        private readonly ILogger<PlatilloController> _logger;

        public PlatilloController(IPlatilloRepository platilloRepo, ICategoriaRepository categoriaRepo, IFileService fileService, ILogger<PlatilloController> logger)
        {
            _platilloRepo = platilloRepo;
            _categoriaRepo = categoriaRepo;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<IActionResult> Platillo()
        {
            _logger.LogInformation("Cargando lista de platillos");
            var platillos = await _platilloRepo.GetPlatillos();
            return View(platillos);
        }

        public async Task<IActionResult> AddPlatillo()
        {
            _logger.LogInformation("Iniciando vista para agregar platillo");
            var categoriaSelectList = (await _categoriaRepo.GetCategoria()).Select(categoria => new SelectListItem
            {
                Text = categoria.CategoriaName,
                Value = categoria.Id.ToString(),
            });
            PlatilloDTO platilloToAdd = new() { CategoriaList = categoriaSelectList };
            return View(platilloToAdd);
        }

        [HttpPost]
        public async Task<IActionResult> AddPlatillo(PlatilloDTO platilloToAdd)
        {
            _logger.LogInformation("Procesando solicitud para agregar un nuevo platillo");
            var categoriaSelectList = (await _categoriaRepo.GetCategoria()).Select(categoria => new SelectListItem
            {
                Text = categoria.CategoriaName,
                Value = categoria.Id.ToString(),
            });
            platilloToAdd.CategoriaList = categoriaSelectList;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al intentar agregar platillo");
                return View(platilloToAdd);
            }

            try
            {
                if (platilloToAdd.ImageFile != null)
                {
                    if (platilloToAdd.ImageFile.Length > 1 * 1024 * 1024)
                        throw new InvalidOperationException("La imagen no debe sobrepasar 1 MB");

                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(platilloToAdd.ImageFile, allowedExtensions);
                    platilloToAdd.ImageURL = imageName;
                    _logger.LogInformation("Imagen guardada correctamente: {Imagen}", imageName);
                }

                Platillo platillo = new()
                {
                    Id = platilloToAdd.Id,
                    PlatilloName = platilloToAdd.PlatilloName,
                    ImagenUrl = platilloToAdd.ImageURL,
                    CategoriaId = platilloToAdd.CategoriaId,
                    Precio = platilloToAdd.Precio,
                    Descripcion = platilloToAdd.Descripcion
                };

                await _platilloRepo.AddPlatillo(platillo);
                _logger.LogInformation("Platillo agregado exitosamente: {Nombre}", platillo.PlatilloName);
                TempData["successMessage"] = "El platillo se ha añadido exitosamente";
                return RedirectToAction(nameof(AddPlatillo));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al agregar platillo: {Mensaje}", ex.Message);
                TempData["errorMessage"] = ex.Message.Contains("imagen") ? ex.Message : "Error al guardar";
                return View(platilloToAdd);
            }
        }

        public async Task<IActionResult> UpdatePlatillo(int id)
        {
            _logger.LogInformation("Iniciando actualización del platillo con id {Id}", id);
            var platillo = await _platilloRepo.GetPlatilloById(id);
            if (platillo == null)
            {
                _logger.LogWarning("Platillo con id {Id} no encontrado", id);
                TempData["errorMessage"] = $"Platillo con el id: {id} no ha sido encontrado";
                return RedirectToAction(nameof(Platillo));
            }

            var categoriaSelectList = (await _categoriaRepo.GetCategoria()).Select(categoria => new SelectListItem
            {
                Text = categoria.CategoriaName,
                Value = categoria.Id.ToString(),
                Selected = categoria.Id == platillo.CategoriaId
            });

            PlatilloDTO platilloToUpdate = new()
            {
                CategoriaList = categoriaSelectList,
                PlatilloName = platillo.PlatilloName,
                CategoriaId = platillo.CategoriaId,
                Descripcion = platillo.Descripcion,
                Precio = platillo.Precio,
                ImageURL = platillo.ImagenUrl
            };

            return View(platilloToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePlatillo(PlatilloDTO platilloToUpdate)
        {
            _logger.LogInformation("Procesando actualización para el platillo con id {Id}", platilloToUpdate.Id);
            var categoriaSelectList = (await _categoriaRepo.GetCategoria()).Select(categoria => new SelectListItem
            {
                Text = categoria.CategoriaName,
                Value = categoria.Id.ToString(),
                Selected = categoria.Id == platilloToUpdate.CategoriaId
            });
            platilloToUpdate.CategoriaList = categoriaSelectList;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al intentar actualizar platillo con id {Id}", platilloToUpdate.Id);
                return View(platilloToUpdate);
            }

            try
            {
                string oldImage = "";
                if (platilloToUpdate.ImageFile != null)
                {
                    if (platilloToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                        throw new InvalidOperationException("La imagen no puede sobrepasar 1 MB");

                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(platilloToUpdate.ImageFile, allowedExtensions);
                    oldImage = platilloToUpdate.ImageURL;
                    platilloToUpdate.ImageURL = imageName;
                    _logger.LogInformation("Imagen actualizada correctamente: {Imagen}", imageName);
                }

                Platillo platillo = new()
                {
                    Id = platilloToUpdate.Id,
                    PlatilloName = platilloToUpdate.PlatilloName,
                    CategoriaId = platilloToUpdate.CategoriaId,
                    Descripcion = platilloToUpdate.Descripcion,
                    Precio = platilloToUpdate.Precio,
                    ImagenUrl = platilloToUpdate.ImageURL
                };

                await _platilloRepo.UpdatePlatillo(platillo);
                _logger.LogInformation("Platillo actualizado correctamente: {Nombre}", platillo.PlatilloName);

                if (!string.IsNullOrWhiteSpace(oldImage))
                {
                    _fileService.DeleteFile(oldImage);
                    _logger.LogInformation("Imagen anterior eliminada: {Imagen}", oldImage);
                }

                TempData["successMessage"] = "Platillo actualizado correctamente";
                return RedirectToAction(nameof(Platillo));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar platillo con id {Id}: {Mensaje}", platilloToUpdate.Id, ex.Message);
                TempData["errorMessage"] = ex.Message.Contains("imagen") ? ex.Message : "Error al guardar";
                return View(platilloToUpdate);
            }
        }

        public async Task<IActionResult> DeletePlatillo(int id)
        {
            _logger.LogInformation("Intentando eliminar platillo con id {Id}", id);
            try
            {
                var platillo = await _platilloRepo.GetPlatilloById(id);
                if (platillo == null)
                {
                    _logger.LogWarning("Platillo con id {Id} no encontrado para eliminar", id);
                    TempData["errorMessage"] = $"El platillo con el id: {id} no ha sido encontrado";
                }
                else
                {
                    await _platilloRepo.DeletePlatillo(platillo);
                    _logger.LogInformation("Platillo eliminado correctamente: {Nombre}", platillo.PlatilloName);

                    if (!string.IsNullOrWhiteSpace(platillo.ImagenUrl))
                    {
                        _fileService.DeleteFile(platillo.ImagenUrl);
                        _logger.LogInformation("Imagen asociada eliminada: {Imagen}", platillo.ImagenUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar platillo con id {Id}: {Mensaje}", id, ex.Message);
                TempData["errorMessage"] = "Error al eliminar el platillo";
            }

            return RedirectToAction(nameof(Platillo));
        }
    }
}
