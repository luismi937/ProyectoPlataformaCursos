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
        public async Task<IActionResult> Inscribir(
            int idCurso,
            string? metodoPago,
            string? cardHolderName,
            string? cardNumber,
            string? cardExpiry,
            string? cardCvc,
            string? transferAccountHolder,
            string? transferIban,
            string? transferBankName,
            string? transferReference)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var resultado = await _enrollmentService.InscribirUsuarioAsync(
                userId,
                idCurso,
                metodoPago,
                cardHolderName,
                cardNumber,
                cardExpiry,
                cardCvc,
                transferAccountHolder,
                transferIban,
                transferBankName,
                transferReference);

            if (resultado.Success)
            {
                TempData["Success"] = resultado.Message;
            }
            else
            {
                TempData["Error"] = resultado.Message;
            }

            return RedirectToAction("Index", "Course");
        }
    }
}
