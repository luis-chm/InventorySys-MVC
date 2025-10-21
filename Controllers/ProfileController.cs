using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventorySys.Models;
using System.Security.Claims;

namespace InventorySys.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly InventorySysContext _context;

        public ProfileController(InventorySysContext context)
        {
            _context = context;
        }
        // GET: Profile
        public async Task<IActionResult> Index()
        {
            //Obtener el ID del usuario logueado
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //Buscar el usuario con su rol
            var user = await _context.TblUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
