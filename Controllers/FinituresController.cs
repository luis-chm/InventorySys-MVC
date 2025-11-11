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
    public class FinituresController : Controller
    {
        private readonly InventorySysContext _context;

        public FinituresController(InventorySysContext context)
        {
            _context = context;
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        // GET: Finitures
        public async Task<IActionResult> Index()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanAccessModule(userRole, RolePermissionHelper.SystemModule.Finitures))
                return RedirectToAction("AccessDenied", "Home");

            return View(await _context.TblFinitures.ToListAsync());
        }

        // GET: Finitures/CreateModal
        public IActionResult CreateModal()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Finitures))
                return NotFound();

            return PartialView("Create", new TblFiniture());
        }

        // POST: Finitures/CreateModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal([Bind("FinitureId,FinitureCode,FinitureName,FinitureActive")] TblFiniture tblFiniture)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Finitures))
                return Json(new { success = false, message = "No tienes permiso para crear acabados." });

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tblFiniture);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Acabado creado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return PartialView("Create", tblFiniture);
        }

        // GET: Finitures/EditModal/5
        public async Task<IActionResult> EditModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Finitures))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblFiniture = await _context.TblFinitures.FindAsync(id);
            if (tblFiniture == null)
                return NotFound();

            return PartialView("Edit", tblFiniture);
        }

        // POST: Finitures/EditModal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, [Bind("FinitureId,FinitureCode,FinitureName,FinitureActive")] TblFiniture tblFiniture)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Finitures))
                return Json(new { success = false, message = "No tienes permiso para editar acabados." });

            if (id != tblFiniture.FinitureId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblFiniture);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Acabado actualizado exitosamente." });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblFinitureExists(tblFiniture.FinitureId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return PartialView("Edit", tblFiniture);
        }

        // GET: Finitures/DeleteModal/5
        public async Task<IActionResult> DeleteModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Finitures))
                return NotFound();

            if (id == null)
                return NotFound();

            var tblFiniture = await _context.TblFinitures
                .FirstOrDefaultAsync(m => m.FinitureId == id);

            if (tblFiniture == null)
                return NotFound();

            return PartialView("Delete", tblFiniture);
        }

        // POST: Finitures/DeleteModal
        [HttpPost, ActionName("DeleteModal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = GetUserRole();
            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Finitures))
                return Json(new { success = false, message = "No tienes permiso para eliminar acabados." });

            try
            {
                var tblFiniture = await _context.TblFinitures
                    .Include(f => f.TblMaterials)
                    .FirstOrDefaultAsync(f => f.FinitureId == id);

                if (tblFiniture == null)
                    return Json(new { success = false, message = "El acabado no existe." });

                // Verificar si el acabado tiene materiales asociados
                if (tblFiniture.TblMaterials.Any())
                    return Json(new { success = false, message = "No puedes eliminar el acabado. Tiene materiales asociados." });

                _context.TblFinitures.Remove(tblFiniture);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Acabado eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }

        private bool TblFinitureExists(int id)
        {
            return _context.TblFinitures.Any(e => e.FinitureId == id);
        }
    }
}