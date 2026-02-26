using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
using ProyectoPlataformaCursos.Models;

namespace ProyectoPlataformaCursos.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var totalUsuarios = await _context.Usuarios.CountAsync();
            var totalCursos = await _context.Cursos.CountAsync();
            var totalInscripciones = await _context.Inscripciones.CountAsync();
            
            ViewBag.TotalUsuarios = totalUsuarios;
            ViewBag.TotalCursos = totalCursos;
            ViewBag.TotalInscripciones = totalInscripciones;

            return View();
        }

        public async Task<IActionResult> Usuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Cursos()
        {
            var cursos = await _context.Cursos
                .Include(c => c.Profesor)
                .Include(c => c.Lecciones)
                .ToListAsync();
            return View(cursos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCursoActivo(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                curso.Activo = !curso.Activo;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Curso {(curso.Activo ? "activado" : "desactivado")} exitosamente";
            }
            return RedirectToAction(nameof(Cursos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario != null)
            {
                await _userManager.DeleteAsync(usuario);
                TempData["Success"] = "Usuario eliminado exitosamente";
            }
            return RedirectToAction(nameof(Usuarios));
        }
    }
}
