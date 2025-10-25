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
    public class CollectionsController : Controller
    {
        private readonly InventorySysContext _context;

        public CollectionsController(InventorySysContext context)
        {
            _context = context;
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        // GET: Collections
        public async Task<IActionResult> Index()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanAccessModule(userRole, RolePermissionHelper.SystemModule.Collections))
                return RedirectToAction("AccessDenied", "Home");

            return View(await _context.TblCollections.ToListAsync());
        }

        // GET: Collections/CreateModal
        public IActionResult CreateModal()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Collections))
                return NotFound();

            return PartialView("Create", new TblCollection());
        }

        // POST: Collections/CreateModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal([Bind("CollectionId,CollectionName,CollectionEffect,CollectionActive")] TblCollection tblCollection)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Collections))
                return Json(new { success = false, message = "No tienes permiso para crear colecciones." });

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tblCollection);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Colección creada exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return PartialView("Create", tblCollection);
        }

        // GET: Collections/EditModal/5
        public async Task<IActionResult> EditModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Collections))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblCollection = await _context.TblCollections.FindAsync(id);
            if (tblCollection == null)
                return NotFound();

            return PartialView("Edit", tblCollection);
        }

        // POST: Collections/EditModal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, [Bind("CollectionId,CollectionName,CollectionEffect,CollectionActive")] TblCollection tblCollection)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Collections))
                return Json(new { success = false, message = "No tienes permiso para editar colecciones." });

            if (id != tblCollection.CollectionId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblCollection);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Colección actualizada exitosamente." });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblCollectionExists(tblCollection.CollectionId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return PartialView("Edit", tblCollection);
        }

        // GET: Collections/DeleteModal/5
        public async Task<IActionResult> DeleteModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Collections))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblCollection = await _context.TblCollections.FirstOrDefaultAsync(m => m.CollectionId == id);
            if (tblCollection == null)
                return NotFound();

            return PartialView("Delete", tblCollection);
        }

        // POST: Collections/DeleteModal/5
        [HttpPost, ActionName("DeleteModal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Collections))
                return Json(new { success = false, message = "No tienes permiso para eliminar colecciones." });

            try
            {
                var tblCollection = await _context.TblCollections.FindAsync(id);
                if (tblCollection != null)
                {
                    _context.TblCollections.Remove(tblCollection);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Colección eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool TblCollectionExists(int id)
        {
            return _context.TblCollections.Any(e => e.CollectionId == id);
        }
    }
}