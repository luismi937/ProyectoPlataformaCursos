using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoPlataformaCursos.Models;
using ProyectoPlataformaCursos.Models.ViewModels;
using System.Security.Claims;

namespace ProyectoPlataformaCursos.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public AccountController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Role, user.Rol),
                            new Claim(ClaimTypes.Name, $"{user.Nombre} {user.Apellidos}"),
                            new Claim(ClaimTypes.Email, user.Email)
                        };

                        await _signInManager.SignInWithClaimsAsync(user, model.RememberMe, claims);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado");
                    return View(model);
                }

                var usuario = new Usuario
                {
                    Nombre = model.Nombre,
                    Apellidos = model.Apellidos,
                    Email = model.Email,
                    Rol = model.Rol,
                    FechaRegistro = DateTime.Now
                };

                var result = await _userManager.CreateAsync(usuario, model.Password);

                if (result.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, usuario.Rol),
                        new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellidos}"),
                        new Claim(ClaimTypes.Email, usuario.Email)
                    };

                    await _signInManager.SignInWithClaimsAsync(usuario, isPersistent: false, claims);
                    TempData["Success"] = "Registro exitoso. Bienvenido!";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
