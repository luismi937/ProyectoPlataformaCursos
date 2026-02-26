using Microsoft.AspNetCore.Mvc;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace ProyectoPlataformaCursos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CourseService _courseService;

        public HomeController(ILogger<HomeController> logger, CourseService courseService)
        {
            _logger = logger;
            _courseService = courseService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

            var cursos = await _courseService.GetAllCursosAsync(userId);
            ViewBag.CursosDestacados = cursos.Take(3);

            return View();
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
