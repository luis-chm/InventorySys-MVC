using InventorySys.Helpers;
using InventorySys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventorySys.Controllers
{
    [Authorize]
    public class FormatsController : Controller
    {
        private readonly InventorySysContext _context;

        public FormatsController(InventorySysContext context)
        {
            _context = context;
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        // GET: Formats
        public async Task<IActionResult> Index()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanAccessModule(userRole, RolePermissionHelper.SystemModule.Formats))
                return RedirectToAction("AccessDenied", "Home");

            return View(await _context.TblFormats.ToListAsync());
        }

        // GET: Formats/CreateModal
        public IActionResult CreateModal()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Formats))
                return NotFound();

            return PartialView("Create", new TblFormat());
        }

        // POST: Formats/CreateModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal([Bind("FormatId,FormatName,FormatSizeCm,FormatActive")] TblFormat tblFormat)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Formats))
                return Json(new { success = false, message = "No tienes permiso para crear formatos." });

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tblFormat);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Formato creado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return PartialView("Create", tblFormat);
        }

        // GET: Formats/EditModal/5
        public async Task<IActionResult> EditModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Formats))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblFormat = await _context.TblFormats.FindAsync(id);
            if (tblFormat == null)
                return NotFound();

            return PartialView("Edit", tblFormat);
        }

        // POST: Formats/EditModal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, [Bind("FormatId,FormatName,FormatSizeCm,FormatActive")] TblFormat tblFormat)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Formats))
                return Json(new { success = false, message = "No tienes permiso para editar formatos." });

            if (id != tblFormat.FormatId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblFormat);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Formato actualizado exitosamente." });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblFormatExists(tblFormat.FormatId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return PartialView("Edit", tblFormat);
        }

        // GET: Formats/DeleteModal/5
        public async Task<IActionResult> DeleteModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Formats))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblFormat = await _context.TblFormats.FirstOrDefaultAsync(m => m.FormatId == id);
            if (tblFormat == null)
                return NotFound();

            return PartialView("Delete", tblFormat);
        }

        // POST: Formats/DeleteModal/5
        [HttpPost, ActionName("DeleteModal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Formats))
                return Json(new { success = false, message = "No tienes permiso para eliminar formatos." });

            try
            {
                var tblFormat = await _context.TblFormats.FindAsync(id);
                if (tblFormat != null)
                {
                    _context.TblFormats.Remove(tblFormat);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Formato eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool TblFormatExists(int id)
        {
            return _context.TblFormats.Any(e => e.FormatId == id);
        }
    }
}