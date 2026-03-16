using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
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
                    var passwordOk = await _userManager.CheckPasswordAsync(user, model.Password);

                    if (passwordOk)
                    {
                        await SignInUsuarioAsync(user, model.RememberMe);

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
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpperInvariant(),
                    NormalizedEmail = model.Email.ToUpperInvariant(),
                    Rol = model.Rol,
                    FechaRegistro = DateTime.Now
                };

                var result = await _userManager.CreateAsync(usuario, model.Password);

                if (result.Succeeded)
                {
                    await SignInUsuarioAsync(usuario, rememberMe: false);
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

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            if (TempData.ContainsKey("controller") && TempData.ContainsKey("action"))
            {
                string? controller = TempData["controller"]?.ToString();
                string? action = TempData["action"]?.ToString();

                if (!string.IsNullOrEmpty(controller) && !string.IsNullOrEmpty(action))
                {
                    if (TempData.ContainsKey("id"))
                    {
                        string? id = TempData["id"]?.ToString();
                        return RedirectToAction(action, controller, new { id = id });
                    }
                    else
                    {
                        return RedirectToAction(action, controller);
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [ProyectoPlataformaCursos.Filters.AuthorizeUsuarios]
        public IActionResult Perfil()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SignInUsuarioAsync(Usuario usuario, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellidos}"),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim("FechaRegistro", usuario.FechaRegistro.ToString("yyyy-MM-dd"))
            };

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme, ClaimTypes.Name, ClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
            });
        }
    }
}
