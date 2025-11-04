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

        public PlatilloController(IPlatilloRepository platilloRepo, ICategoriaRepository categoriaRepo, IFileService fileService)
        {
            _platilloRepo = platilloRepo;
            _categoriaRepo = categoriaRepo;
            _fileService = fileService;
        }

        public async Task<IActionResult> Platillo()
        {
            var platillos = await _platilloRepo.GetPlatillos();
            return View(platillos);
        }

        public async Task<IActionResult> AddPlatillo()
        {
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
            var categoriaSelectList = (await _categoriaRepo.GetCategoria()).Select(categoria => new SelectListItem
            {
                Text = categoria.CategoriaName,
                Value = categoria.Id.ToString(),
            });
            platilloToAdd.CategoriaList = categoriaSelectList;

            if (!ModelState.IsValid)
                return View(platilloToAdd);

            try
            {
                if (platilloToAdd.ImageFile != null)
                {
                    if (platilloToAdd.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("La imagen no debe sobrepasar 1 MB");
                    }
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(platilloToAdd.ImageFile, allowedExtensions);
                    platilloToAdd.ImageURL = imageName;
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
                TempData["successMessage"] = "El patillo se ha añadido exitosamente";
                return RedirectToAction(nameof(AddPlatillo));
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(platilloToAdd);
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(platilloToAdd);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error al guardar";
                return View(platilloToAdd);
            }
        }

        public async Task<IActionResult> UpdatePlatillo(int id)
        {
            var platillo = await _platilloRepo.GetPlatilloById(id);
            if (platillo == null)
            {
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
            var categoriaSelectList = (await _categoriaRepo.GetCategoria()).Select(categoria => new SelectListItem
            {
                Text = categoria.CategoriaName,
                Value = categoria.Id.ToString(),
                Selected = categoria.Id == platilloToUpdate.CategoriaId
            });
            platilloToUpdate.CategoriaList = categoriaSelectList;

            if (!ModelState.IsValid)
                return View(platilloToUpdate);

            try
            {
                string oldImage = "";
                if (platilloToUpdate.ImageFile != null)
                {
                    if (platilloToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("La imagen no puede sobrepasar 1 MB");
                    }
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(platilloToUpdate.ImageFile, allowedExtensions);
                    oldImage = platilloToUpdate.ImageURL;
                    platilloToUpdate.ImageURL = imageName;
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
                if (!string.IsNullOrWhiteSpace(oldImage))
                {
                    _fileService.DeleteFile(oldImage);
                }
                TempData["successMessage"] = "Platillo actualizado correctamente";
                return RedirectToAction(nameof(Platillo));
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(platilloToUpdate);
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(platilloToUpdate);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error al guardar";
                return View(platilloToUpdate);
            }
        }

        public async Task<IActionResult> DeletePlatillo(int id)
        {
            try
            {
                var platillo = await _platilloRepo.GetPlatilloById(id);
                if (platillo == null)
                {
                    TempData["errorMessage"] = $"El platillo con el id: {id} no ha sido encontrado";
                }
                else
                {
                    await _platilloRepo.DeletePlatillo(platillo);
                    if (!string.IsNullOrWhiteSpace(platillo.ImagenUrl))
                    {
                        _fileService.DeleteFile(platillo.ImagenUrl);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error al eliminar el platillo";
            }
            return RedirectToAction(nameof(Platillo));
        }

    }
}


