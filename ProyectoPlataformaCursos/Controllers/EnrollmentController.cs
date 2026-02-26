using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoPlataformaCursos.Services;
using System.Security.Claims;

namespace ProyectoPlataformaCursos.Controllers
{
    [Authorize(Roles = "ALUMNO")]
    public class EnrollmentController : Controller
    {
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentController(EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        public async Task<IActionResult> MisCursos()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cursos = await _enrollmentService.GetMisCursosAsync(userId);
            return View(cursos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscribir(int idCurso)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var resultado = await _enrollmentService.InscribirUsuarioAsync(userId, idCurso);

            if (resultado)
            {
                TempData["Success"] = "Te has inscrito exitosamente al curso";
            }
            else
            {
                TempData["Error"] = "Ya estás inscrito en este curso";
            }

            return RedirectToAction("Index", "Course");
        }
    }
}
