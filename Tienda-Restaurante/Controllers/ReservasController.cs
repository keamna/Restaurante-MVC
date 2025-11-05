using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tienda_Restaurante.Areas.Identity.Data;
using Tienda_Restaurante.Models;
using Tienda_Restaurante.Services;

namespace Tienda_Restaurante.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ReservasController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ICuerpoCorreoService _cuerpoCorreo;

        public ReservasController(ApplicationDbContext db, UserManager<IdentityUser> userManager, ILogger<ReservasController> logger, IEmailSender emailSender, ICuerpoCorreoService cuerpoCorreo)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _cuerpoCorreo = cuerpoCorreo;

        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Acceso a la vista de creación de reserva.");
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Reserva modelo)
        {
            _logger.LogInformation("Intento de creación de reserva.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Datos inválidos en la reserva.");
                TempData["error"] = "Datos inválidos en la reserva.";
                return View(modelo);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Intento de reserva sin usuario autenticado.");
                TempData["error"] = "Debes iniciar sesión para reservar.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            modelo.UsuarioId = user.Id;
            modelo.Fecha = modelo.Fecha.Date;

            if (modelo.HoraFin <= modelo.HoraInicio)
            {
                _logger.LogWarning("Hora de fin anterior o igual a la hora de inicio.");
                TempData["error"] = "La hora de fin debe ser posterior al inicio.";
                return View(modelo);
            }

            try
            {
                _db.Reservas.Add(modelo);
                await _db.SaveChangesAsync();
                var cuerpo = _cuerpoCorreo.GenerarCuerpoReserva(new List<Reserva> { modelo });
                await _emailSender.SendEmailAsync(user.Email, "Confirmación de Reserva", cuerpo);
                _logger.LogInformation("Correo de confirmación de reserva enviado a {Correo}", user.Email);

                _logger.LogInformation("Reserva creada correctamente para el usuario {UsuarioId}.", user.Id);
                TempData["success"] = "Reserva creada correctamente.";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la reserva para el usuario {UsuarioId}.", user.Id);
                TempData["error"] = "Error al guardar la reserva.";
                return View(modelo);
            }
        }

        [AllowAnonymous]
        public IActionResult Gracias()
        {
            _logger.LogInformation("Vista de agradecimiento mostrada.");
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Carga de la lista de reservas.");
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
            _logger.LogInformation("Intento de cambio de estado de la reserva {ReservaId} a {Estado}.", id, estado);

            var reserva = await _db.Reservas.FindAsync(id);
            if (reserva != null)
            {
                reserva.Estado = estado;
                await _db.SaveChangesAsync();
                _logger.LogInformation("Estado de la reserva {ReservaId} actualizado correctamente.", id);
                TempData["msg"] = "Estado actualizado correctamente.";
            }
            else
            {
                _logger.LogWarning("No se encontró la reserva con ID {ReservaId} para actualizar.", id);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            _logger.LogInformation("Intento de eliminación de la reserva {ReservaId}.", id);

            var reserva = await _db.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _db.Reservas.Remove(reserva);
                await _db.SaveChangesAsync();
                _logger.LogInformation("Reserva {ReservaId} eliminada correctamente.", id);
                TempData["msg"] = "Reserva eliminada.";
            }
            else
            {
                _logger.LogWarning("No se encontró la reserva con ID {ReservaId} para eliminar.", id);
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
