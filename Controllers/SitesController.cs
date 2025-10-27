using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventorySys.Models;
using InventorySys.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InventorySys.Controllers
{
    [Authorize]
    public class SitesController : Controller
    {
        private readonly InventorySysContext _context;

        public SitesController(InventorySysContext context)
        {
            _context = context;
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        // GET: Sites
        public async Task<IActionResult> Index()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanAccessModule(userRole, RolePermissionHelper.SystemModule.Sites))
                return RedirectToAction("AccessDenied", "Home");

            return View(await _context.TblSites.ToListAsync());
        }

        // GET: Sites/CreateModal
        public IActionResult CreateModal()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Sites))
                return NotFound();

            return PartialView("Create", new TblSite());
        }

        // POST: Sites/CreateModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal([Bind("SiteId,SiteName,SiteLocation,SiteActive")] TblSite tblSite)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Sites))
                return Json(new { success = false, message = "No tienes permiso para crear sitios." });

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tblSite);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Sitio creado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return PartialView("Create", tblSite);
        }

        // GET: Sites/EditModal/5
        public async Task<IActionResult> EditModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Sites))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblSite = await _context.TblSites.FindAsync(id);
            if (tblSite == null)
                return NotFound();

            return PartialView("Edit", tblSite);
        }

        // POST: Sites/EditModal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, [Bind("SiteId,SiteName,SiteLocation,SiteActive")] TblSite tblSite)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Sites))
                return Json(new { success = false, message = "No tienes permiso para editar sitios." });

            if (id != tblSite.SiteId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblSite);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Sitio actualizado exitosamente." });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblSiteExists(tblSite.SiteId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return PartialView("Edit", tblSite);
        }

        // GET: Sites/DeleteModal/5
        public async Task<IActionResult> DeleteModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Sites))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblSite = await _context.TblSites.FirstOrDefaultAsync(m => m.SiteId == id);
            if (tblSite == null)
                return NotFound();

            return PartialView("Delete", tblSite);
        }

        // POST: Sites/DeleteModal/5
        [HttpPost, ActionName("DeleteModal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Sites))
                return Json(new { success = false, message = "No tienes permiso para eliminar sitios." });

            try
            {
                var tblSite = await _context.TblSites.FindAsync(id);
                if (tblSite != null)
                {
                    _context.TblSites.Remove(tblSite);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Sitio eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool TblSiteExists(int id)
        {
            return _context.TblSites.Any(e => e.SiteId == id);
        }
    }
}