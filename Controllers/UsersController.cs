using InventorySys.Models;
using InventorySys.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace InventorySys.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly InventorySysContext _context;

        public UsersController(InventorySysContext context)
        {
            _context = context;
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        public async Task<IActionResult> Index()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanAccessModule(userRole, RolePermissionHelper.SystemModule.Users))
                return RedirectToAction("AccessDenied", "Home");

            var inventorySysContext = _context.TblUsers.Include(t => t.Role);
            return View(await inventorySysContext.ToListAsync());
        }

        private bool TblUserExists(int id)
        {
            return _context.TblUsers.Any(e => e.UserId == id);
        }

        // GET: Users/CreateModal
        public IActionResult CreateModal()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Users))
                return NotFound();

            var roles = _context.TblRoles.Where(r => r.RoleActive == true).ToList();
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName", roles.FirstOrDefault()?.RoleId);
            return PartialView("Create", new TblUser());
        }

        //POST: Users/CreateModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal([Bind("UserId,UserName,UserEmail,UserEncryptedPassword,RoleId,UserActive")] TblUser tblUser)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Users))
                return Json(new { success = false, message = "No tienes permiso para crear usuarios." });
            // Validar que el email sea unico
            var emailExists = await _context.TblUsers
                .AnyAsync(u => u.UserEmail == tblUser.UserEmail);

            if (emailExists)
                return Json(new { success = false, message = "El email ya está registrado. Por favor, ingresa un email único." });
            if (ModelState.IsValid)
            {
                try
                {
                    tblUser.UserEncryptedPassword = _context.EncriptarPassword(tblUser.UserEncryptedPassword);
                    _context.Add(tblUser);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Usuario creado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            ViewBag.RoleId = new SelectList(_context.TblRoles.Where(r => r.RoleActive == true), "RoleId", "RoleName", tblUser.RoleId);
            return PartialView("Create", tblUser);
        }

        // GET: Users/EditModal/5
        public async Task<IActionResult> EditModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Users))
                return NotFound();

            if (id == null) return NotFound();
            var tblUser = await _context.TblUsers.FindAsync(id);
            if (tblUser == null) return NotFound();

            var roles = _context.TblRoles.Where(r => r.RoleActive == true).ToList();
            ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName", tblUser.RoleId);
            return PartialView("Edit", tblUser);
        }
        // POST: Users/EditModal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, [Bind("UserId,UserName,UserEmail,UserEncryptedPassword,RoleId,UserActive")] TblUser tblUser)
        {
            var userRole = GetUserRole();
            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Users))
                return Json(new { success = false, message = "No tienes permiso para editar usuarios." });

            if (id != tblUser.UserId) return NotFound();

            // Validar que el email sea único (excepto el usuario actual)
            var emailExists = await _context.TblUsers
                .AnyAsync(u => u.UserEmail == tblUser.UserEmail && u.UserId != tblUser.UserId);

            if (emailExists)
                return Json(new { success = false, message = "El email ya está registrado. Por favor, ingresa un email único." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return Json(new { success = false, message = "Error: " + string.Join(", ", errors.Select(e => e.ErrorMessage)) });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(tblUser.UserEncryptedPassword))
                    {
                        tblUser.UserEncryptedPassword = _context.EncriptarPassword(tblUser.UserEncryptedPassword);
                    }
                    else
                    {
                        var usuarioActual = await _context.TblUsers.FindAsync(id);
                        tblUser.UserEncryptedPassword = usuarioActual?.UserEncryptedPassword!;
                    }

                    _context.Update(tblUser);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Usuario actualizado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return Json(new { success = false, message = "Error desconocido." });
        }

        // GET: Users/DeleteModal/5
        public async Task<IActionResult> DeleteModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Users))
                return NotFound();

            if (id == null) return NotFound();
            var tblUser = await _context.TblUsers
                .Include(t => t.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (tblUser == null) return NotFound();
            return PartialView("Delete", tblUser);
        }

        // POST: Users/DeleteModalConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModalConfirmed(int id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Users))
                return Json(new { success = false, message = "No tienes permiso para eliminar usuarios." });

            try
            {
                var tblUser = await _context.TblUsers.FindAsync(id);
                if (tblUser != null)
                {
                    _context.TblUsers.Remove(tblUser);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Usuario eliminado exitosamente." });
                }
                return Json(new { success = false, message = "Usuario no encontrado." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}