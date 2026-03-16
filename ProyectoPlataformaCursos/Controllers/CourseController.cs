using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Services;
using ProyectoPlataformaCursos.Filters;
using System.Security.Claims;

namespace ProyectoPlataformaCursos.Controllers
{
    [AuthorizeUsuarios]
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;

        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

            var cursos = await _courseService.GetAllCursosAsync(userId);
            return View(cursos);
        }

        [Authorize(Roles = "PROFESOR,ADMIN")]
        public async Task<IActionResult> MisCursos()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cursos = await _courseService.GetCursosByProfesorAsync(userId);
            return View(cursos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var curso = await _courseService.GetCursoByIdAsync(id);
            if (curso == null)
            {
                return NotFound();
            }
            return View(curso);
        }

        [Authorize(Roles = "PROFESOR,ADMIN")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "PROFESOR,ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                curso.IdProfesor = userId;

                await _courseService.CreateCursoAsync(curso);
                TempData["Success"] = "Curso creado exitosamente";
                return RedirectToAction(nameof(MisCursos));
            }
            return View(curso);
        }

        [Authorize(Roles = "PROFESOR,ADMIN")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _courseService.GetCursoByIdAsync(id);
            if (curso == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (curso.IdProfesor != userId && !User.IsInRole("ADMIN"))
            {
                return Forbid();
            }

            return View(curso);
        }

        [Authorize(Roles = "PROFESOR,ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.IdCurso)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (curso.IdProfesor != userId && !User.IsInRole("ADMIN"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                await _courseService.UpdateCursoAsync(curso);
                TempData["Success"] = "Curso actualizado exitosamente";
                return RedirectToAction(nameof(MisCursos));
            }
            return View(curso);
        }

        [Authorize(Roles = "PROFESOR,ADMIN")]
        [Authorize(Policy = "TieneCursosPolicy")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var curso = await _courseService.GetCursoByIdAsync(id);
            if (curso == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (curso.IdProfesor != userId && !User.IsInRole("ADMIN"))
            {
                return Forbid();
            }

            return View(curso);
        }

        [Authorize(Roles = "PROFESOR,ADMIN")]
        [Authorize(Policy = "TieneCursosPolicy")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _courseService.GetCursoByIdAsync(id);
            if (curso == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (curso.IdProfesor != userId && !User.IsInRole("ADMIN"))
            {
                return Forbid();
            }

            await _courseService.DeleteCursoAsync(id);
            TempData["Success"] = "Curso eliminado exitosamente";
            return RedirectToAction(nameof(MisCursos));
        }
    }
}
