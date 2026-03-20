using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
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
        private readonly ApplicationDbContext _context;

        public CourseController(CourseService courseService, ApplicationDbContext context)
        {
            _courseService = courseService;
            _context = context;
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
        public async Task<IActionResult> Create()
        {
            await CargarProfesoresAsync();
            return View();
        }

        [Authorize(Roles = "PROFESOR,ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            await CargarProfesoresAsync();

            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (!User.IsInRole("ADMIN"))
                {
                    curso.IdProfesor = userId;
                    curso.Precio = 0;
                    curso.AceptaEfectivo = false;
                    curso.AceptaTarjeta = false;
                    curso.AceptaTransferencia = false;
                }
                else
                {
                    var profesorValido = await _context.Usuarios
                        .AnyAsync(u => u.Id == curso.IdProfesor && u.Rol == "PROFESOR");

                    if (!profesorValido)
                    {
                        ModelState.AddModelError(nameof(curso.IdProfesor), "Debes seleccionar un profesor válido.");
                        return View(curso);
                    }
                }

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

            await CargarProfesoresAsync();

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

            await CargarProfesoresAsync();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cursoActual = await _courseService.GetCursoByIdAsync(id);
            if (cursoActual == null)
            {
                return NotFound();
            }

            if (cursoActual.IdProfesor != userId && !User.IsInRole("ADMIN"))
            {
                return Forbid();
            }

            if (!User.IsInRole("ADMIN"))
            {
                curso.IdProfesor = userId;
            }
            else
            {
                var profesorValido = await _context.Usuarios
                    .AnyAsync(u => u.Id == curso.IdProfesor && u.Rol == "PROFESOR");

                if (!profesorValido)
                {
                    ModelState.AddModelError(nameof(curso.IdProfesor), "Debes seleccionar un profesor válido.");
                    return View(curso);
                }
            }

            if (ModelState.IsValid)
            {
                await _courseService.UpdateCursoAsync(curso);
                TempData["Success"] = "Curso actualizado exitosamente";
                return RedirectToAction(nameof(MisCursos));
            }
            return View(curso);
        }

        private async Task CargarProfesoresAsync()
        {
            if (!User.IsInRole("ADMIN"))
            {
                return;
            }

            var profesores = await _context.Usuarios
                .Where(u => u.Rol == "PROFESOR")
                .OrderBy(u => u.Nombre)
                .ThenBy(u => u.Apellidos)
                .Select(u => new
                {
                    u.Id,
                    NombreCompleto = $"{u.Nombre} {u.Apellidos}"
                })
                .ToListAsync();

            ViewBag.Profesores = new SelectList(profesores, "Id", "NombreCompleto");
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
