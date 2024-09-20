using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using inmobiliariaAST.Models;
using AuthService = inmobiliariaAST.Services.AuthenticationService;


namespace inmobiliariaAST.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly RepositorioUsuario repo;
        private readonly AuthService _authService;

        public AuthController(ILogger<AuthController> logger, RepositorioUsuario repo, AuthService authService)
        {
            _logger = logger;
            this.repo = repo;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Buscar el usuario por email, asegurando que model.Email no sea nulo
                Usuario? usuario = !string.IsNullOrEmpty(model.Email) ? repo.GetByEmail(model.Email) : null;


                if (usuario != null)
                {
                    // Verificar que usuario.Password y model.Password no sean nulos antes de verificar la contraseña
                    if (!string.IsNullOrEmpty(usuario.Password) && !string.IsNullOrEmpty(model.Password) && 
                        _authService.VerifyPassword(usuario.Password, model.Password))
                    {
                        // Crear claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, usuario.Email ?? string.Empty),
                            new Claim("ID_usuario", usuario.ID_usuario.ToString()),
                            new Claim(ClaimTypes.Role, usuario.Rol == 1 ? "Administrador" : "Empleado"),
                            new Claim("Avatar", usuario.Avatar ?? string.Empty)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true
                        };

                        // Autenticar
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Contraseña incorrecta.";
                    }
                }
                else
                {
                    ViewBag.Error = "Email no encontrado.";
                }
            }
            else
            {
                ViewBag.Error = "Por favor, complete todos los campos.";
            }
            return View();
        }



        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        //acceso den
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}