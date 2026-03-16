using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPlataformaCursos.Data;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Models.ViewModels;

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
            var ingresoTotal = await _context.Inscripciones
                .Select(i => (decimal?)i.ImportePagado)
                .SumAsync() ?? 0m;
            
            ViewBag.TotalUsuarios = totalUsuarios;
            ViewBag.TotalCursos = totalCursos;
            ViewBag.TotalInscripciones = totalInscripciones;
            ViewBag.IngresoTotal = ingresoTotal;

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

        [HttpGet]
        public async Task<IActionResult> ConfigurarCurso(int id)
        {
            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.IdCurso == id);
            if (curso == null)
            {
                return NotFound();
            }

            var model = new AdminCursoPagoViewModel
            {
                IdCurso = curso.IdCurso,
                Titulo = curso.Titulo,
                Precio = curso.Precio,
                Activo = curso.Activo,
                AceptaEfectivo = curso.AceptaEfectivo,
                AceptaTarjeta = curso.AceptaTarjeta,
                AceptaTransferencia = curso.AceptaTransferencia
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurarCurso(AdminCursoPagoViewModel model)
        {
            if (!model.AceptaEfectivo && !model.AceptaTarjeta && !model.AceptaTransferencia && model.Precio > 0)
            {
                ModelState.AddModelError(string.Empty, "Debes habilitar al menos una forma de pago para cursos de pago.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.IdCurso == model.IdCurso);
            if (curso == null)
            {
                return NotFound();
            }

            curso.Titulo = model.Titulo;
            curso.Precio = model.Precio;
            curso.Activo = model.Activo;
            curso.AceptaEfectivo = model.AceptaEfectivo;
            curso.AceptaTarjeta = model.AceptaTarjeta;
            curso.AceptaTransferencia = model.AceptaTransferencia;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Configuración comercial del curso actualizada";
            return RedirectToAction(nameof(Cursos));
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
