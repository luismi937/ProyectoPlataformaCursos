using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Repositories.Interfaces;
using ProyectoPlataformaCursos.Services;
using System.Security.Claims;

namespace ProyectoPlataformaCursos.Controllers
{
    [Authorize]
    public class LessonController : Controller
    {
        private readonly ILeccionRepository _leccionRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly ProgressService _progressService;

        public LessonController(
            ILeccionRepository leccionRepository,
            ICursoRepository cursoRepository,
            ProgressService progressService)
        {
            _leccionRepository = leccionRepository;
            _cursoRepository = cursoRepository;
            _progressService = progressService;
        }

        [Authorize(Roles = "ALUMNO")]
        public async Task<IActionResult> View(int cursoId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var lecciones = await _progressService.GetLeccionesCursoAsync(cursoId, userId);
            
            var curso = await _cursoRepository.GetByIdAsync(cursoId);
            ViewBag.CursoTitulo = curso?.Titulo;
            ViewBag.CursoId = cursoId;

            return View(lecciones);
        }

        [Authorize(Roles = "ALUMNO")]
        public async Task<IActionResult> Details(int id)
        {
            var leccion = await _leccionRepository.GetByIdAsync(id);
            if (leccion == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var lecciones = await _progressService.GetLeccionesCursoAsync(leccion.IdCurso, userId);
            var leccionViewModel = lecciones.FirstOrDefault(l => l.IdLeccion == id);

            return View(leccionViewModel);
        }

        [Authorize(Roles = "ALUMNO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarCompletada(int idLeccion, int idCurso)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _progressService.MarcarLeccionCompletadaAsync(userId, idLeccion);
            
            TempData["Success"] = "Lección marcada como completada";
            return RedirectToAction(nameof(View), new { cursoId = idCurso });
        }

        [Authorize(Roles = "PROFESOR")]
        [HttpGet]
        public async Task<IActionResult> Create(int cursoId)
        {
            var curso = await _cursoRepository.GetByIdAsync(cursoId);
            if (curso == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (curso.IdProfesor != userId && !User.IsInRole("ADMIN"))
            {
                return Forbid();
            }

            ViewBag.CursoId = cursoId;
            ViewBag.CursoTitulo = curso.Titulo;
            return View();
        }

        [Authorize(Roles = "PROFESOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leccion leccion)
        {
            if (ModelState.IsValid)
            {
                var curso = await _cursoRepository.GetByIdAsync(leccion.IdCurso);
                if (curso == null)
                {
                    return NotFound();
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                if (curso.IdProfesor != userId && !User.IsInRole("ADMIN"))
                {
                    return Forbid();
                }

                leccion.FechaCreacion = DateTime.Now;
                await _leccionRepository.CreateAsync(leccion);
                
                TempData["Success"] = "Lección creada exitosamente";
                return RedirectToAction("Details", "Course", new { id = leccion.IdCurso });
            }

            ViewBag.CursoId = leccion.IdCurso;
            return View(leccion);
        }

        [Authorize(Roles = "PROFESOR")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var leccion = await _leccionRepository.GetByIdAsync(id);
            if (leccion == null)
            {
                return NotFound();
            }

            var curso = await _cursoRepository.GetByIdAsync(leccion.IdCurso);
            if (curso == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (curso.IdProfesor != userId && !User.IsInRole("ADMIN"))
            {
                return Forbid();
            }

            ViewBag.CursoTitulo = curso.Titulo;
            return View(leccion);
        }

        [Authorize(Roles = "PROFESOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Leccion leccion)
        {
            if (id != leccion.IdLeccion)
            {
                return NotFound();
            }

            var curso = await _cursoRepository.GetByIdAsync(leccion.IdCurso);
            if (curso == null)
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
                await _leccionRepository.UpdateAsync(leccion);
                TempData["Success"] = "Lección actualizada exitosamente";
                return RedirectToAction("Details", "Course", new { id = leccion.IdCurso });
            }

            ViewBag.CursoTitulo = curso.Titulo;
            return View(leccion);
        }

        [Authorize(Roles = "PROFESOR")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var leccion = await _leccionRepository.GetByIdAsync(id);
            if (leccion == null)
            {
                return NotFound();
            }

            var curso = await _cursoRepository.GetByIdAsync(leccion.IdCurso);
            if (curso == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (curso.IdProfesor != userId && !User.IsInRole("ADMIN"))
            {
                return Forbid();
            }

            return View(leccion);
        }

        [Authorize(Roles = "PROFESOR")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leccion = await _leccionRepository.GetByIdAsync(id);
            if (leccion == null)
            {
                return NotFound();
            }

            var curso = await _cursoRepository.GetByIdAsync(leccion.IdCurso);
            if (curso == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (curso.IdProfesor != userId && !User.IsInRole("ADMIN"))
            {
                return Forbid();
            }

            var cursoId = leccion.IdCurso;
            await _leccionRepository.DeleteAsync(id);
            
            TempData["Success"] = "Lección eliminada exitosamente";
            return RedirectToAction("Details", "Course", new { id = cursoId });
        }
    }
}
