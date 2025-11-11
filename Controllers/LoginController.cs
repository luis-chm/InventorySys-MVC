using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventorySys.ViewModels;
using InventorySys.Models;
using System.Security.Claims;

namespace InventorySys.Controllers
{
    public class LoginController : Controller
    {
        private readonly InventorySysContext _context;

        public LoginController(InventorySysContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // LINQ directamente - sin agregar método en DbContext
                    var user = await _context.TblUsers
                        .Include(u => u.Role)
                        .FirstOrDefaultAsync(u => u.UserEmail == model.Email && u.UserActive == true);

                    if (user != null)
                    {
                        // Encriptar password ingresado
                        string passwordEncriptado = _context.EncriptarPassword(model.Password);

                        // Comparar passwords encriptados
                        if (passwordEncriptado == user.UserEncryptedPassword)
                        {
                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.UserEmail),
                        new Claim(ClaimTypes.Role, user.Role.RoleName)
                    };

                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                                new AuthenticationProperties { IsPersistent = model.RememberMe }
                            );

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.AlertMessage = "<i class='fas fa-triangle-exclamation me-2'></i>Credenciales incorrectas. Inténtalo de nuevo.";
                        }
                    }
                    else
                    {
                        ViewBag.AlertMessage = "<i class='fas fa-triangle-exclamation me-2'></i>Tu cuenta está desactivada. Contacta al administrador.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.AlertMessage = $"<i class='fas fa-triangle-exclamation me-2'></i>Error: {ex.Message}";
                }
            }

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}

