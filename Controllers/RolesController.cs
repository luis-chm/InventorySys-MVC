using InventorySys.Models;
using InventorySys.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace InventorySys.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly InventorySysContext _context;

        public RolesController(InventorySysContext context)
        {
            _context = context;
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanAccessModule(userRole, RolePermissionHelper.SystemModule.Roles))
                return RedirectToAction("AccessDenied", "Home");

            return View(await _context.TblRoles.ToListAsync());
        }

        private bool TblRoleExists(int id)
        {
            return _context.TblRoles.Any(e => e.RoleId == id);
        }

        // GET: Roles/CreateModal
        public IActionResult CreateModal()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Roles))
                return NotFound();

            return PartialView("Create", new TblRole());
        }

        // POST: Roles/CreateModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal([Bind("RoleId,RoleName,RoleActive")] TblRole tblRole)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Roles))
                return Json(new { success = false, message = "No tienes permiso para crear roles." });

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tblRole);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Rol creado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return PartialView("Create", tblRole);
        }

        // GET: Roles/EditModal/5
        public async Task<IActionResult> EditModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Roles))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblRole = await _context.TblRoles.FindAsync(id);
            if (tblRole == null)
                return NotFound();

            return PartialView("Edit", tblRole);
        }

        // POST: Roles/EditModal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, [Bind("RoleId,RoleName,RoleActive")] TblRole tblRole)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Roles))
                return Json(new { success = false, message = "No tienes permiso para editar roles." });

            if (id != tblRole.RoleId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblRole);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Rol actualizado exitosamente." });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblRoleExists(tblRole.RoleId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return PartialView("Edit", tblRole);
        }

        // GET: Roles/DeleteModal/5
        public async Task<IActionResult> DeleteModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Roles))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblRole = await _context.TblRoles.FirstOrDefaultAsync(m => m.RoleId == id);
            if (tblRole == null)
                return NotFound();

            return PartialView("Delete", tblRole);
        }

        // POST: Roles/DeleteModal/5
        [HttpPost, ActionName("DeleteModal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Roles))
                return Json(new { success = false, message = "No tienes permiso para eliminar roles." });

            try
            {
                var tblRole = await _context.TblRoles.FindAsync(id);
                if (tblRole != null)
                {
                    _context.TblRoles.Remove(tblRole);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Rol eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}