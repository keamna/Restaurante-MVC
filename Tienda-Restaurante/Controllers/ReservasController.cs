using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tienda_Restaurante.Areas.Identity.Data;
using Tienda_Restaurante.Models;

namespace Tienda_Restaurante.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservasController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

   
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Reserva modelo)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Datos inválidos en la reserva.";
                return View(modelo);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "Debes iniciar sesión para reservar.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            modelo.UsuarioId = user.Id;
            modelo.Fecha = modelo.Fecha.Date;

            if (modelo.HoraFin <= modelo.HoraInicio)
            {
                TempData["error"] = "La hora de fin debe ser posterior al inicio.";
                return View(modelo);
            }

            _db.Reservas.Add(modelo);
            await _db.SaveChangesAsync();

            TempData["success"] = "Reserva creada correctamente.";
            return RedirectToAction("Create");

        }


        [AllowAnonymous]
        public IActionResult Gracias() => View();

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var reservas = await _db.Reservas
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.Fecha)
                .ThenBy(r => r.HoraInicio)
                .ToListAsync();

            return View(reservas);
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            var reserva = await _db.Reservas.FindAsync(id);
            if (reserva != null)
            {
                reserva.Estado = estado;
                await _db.SaveChangesAsync();
                TempData["msg"] = "Estado actualizado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var reserva = await _db.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _db.Reservas.Remove(reserva);
                await _db.SaveChangesAsync();
                TempData["msg"] = "Reserva eliminada.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
