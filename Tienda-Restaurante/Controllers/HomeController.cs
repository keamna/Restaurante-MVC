using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tienda_Restaurante.DTOs;
using Tienda_Restaurante.Models;
using Tienda_Restaurante.Repositories;
using Tienda_Restaurante.Models;

namespace Tienda_Restaurante.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;


        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Platillos(string sterm = "", int categoriaId = 0)
        {
            IEnumerable<Platillo> platillos = await _homeRepository.GetPlatillos(sterm, categoriaId);
            IEnumerable<Categoria> categorias = await _homeRepository.Categorias();
            PlatilloDisplayModel platilloModel = new PlatilloDisplayModel
            {
                Platillos = platillos,
                Categorias = categorias,
                STerm = sterm,
                CategoriaId = categoriaId

            };

            return View(platilloModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
