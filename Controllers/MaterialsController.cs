using InventorySys.Helpers;
using InventorySys.Models;
using InventorySys.ViewModels;
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
    public class MaterialsController : Controller
    {
        private readonly InventorySysContext _context;
        private readonly IWebHostEnvironment _environment;

        public MaterialsController(InventorySysContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        public async Task<IActionResult> Index()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanAccessModule(userRole, RolePermissionHelper.SystemModule.Materials))
                return RedirectToAction("AccessDenied", "Home");

            var materials = _context.TblMaterials
                .Include(m => m.Collection)
                .Include(m => m.Finiture)
                .Include(m => m.Format)
                .Include(m => m.Site)
                .Include(m => m.User);

            return View(await materials.ToListAsync());
        }

        public IActionResult CreateModal()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Materials))
                return NotFound();

            ViewData["CollectionId"] = new SelectList(
                _context.TblCollections.Where(c => c.CollectionActive == true),
                "CollectionId",
                "CollectionName");

            ViewData["FinitureId"] = new SelectList(
                _context.TblFinitures.Where(f => f.FinitureActive == true),
                "FinitureId",
                "FinitureName");

            ViewData["FormatId"] = new SelectList(
                _context.TblFormats.Where(fo => fo.FormatActive == true),
                "FormatId",
                "FormatName");

            ViewData["SiteId"] = new SelectList(
                _context.TblSites.Where(s => s.SiteActive == true),
                "SiteId",
                "SiteName");

            ViewData["UserId"] = new SelectList(
                _context.TblUsers.Where(u => u.UserActive == true),
                "UserId",
                "UserName");

            var model = new MaterialViewModel
            {
                MaterialReceivedDate = DateOnly.FromDateTime(DateTime.Now)
            };
            return PartialView("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(MaterialViewModel model)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.Materials))
                return Json(new { success = false, message = "No tienes permiso para crear materiales." });

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.MaterialFile != null && model.MaterialFile.Length > 0)
                    {
                        var uploads = Path.Combine(_environment.WebRootPath, "UploadedImages");
                        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                        var fileName = Path.GetFileName(model.MaterialFile.FileName);
                        var filePath = Path.Combine(uploads, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.MaterialFile.CopyToAsync(stream);
                        }

                        model.MaterialImg = fileName;
                    }

                    var userName = User?.Identity?.Name;
                    var currentUser = await _context.TblUsers
                        .FirstOrDefaultAsync(u => u.UserName == userName);

                    var entity = new TblMaterial
                    {
                        MaterialCode = model.MaterialCode,
                        MaterialDescription = model.MaterialDescription,
                        CollectionId = model.CollectionId,
                        FinitureId = model.FinitureId,
                        FormatId = model.FormatId,
                        SiteId = model.SiteId,
                        MaterialImg = model.MaterialImg,
                        MaterialReceivedDate = model.MaterialReceivedDate,
                        MaterialStock = model.MaterialStock,
                        UserId = currentUser?.UserId ?? 0,
                    };

                    _context.Add(entity);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Material creado correctamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            ViewData["CollectionId"] = new SelectList(_context.TblCollections, "CollectionId", "CollectionName", model.CollectionId);
            ViewData["FinitureId"] = new SelectList(_context.TblFinitures, "FinitureId", "FinitureName", model.FinitureId);
            ViewData["FormatId"] = new SelectList(_context.TblFormats, "FormatId", "FormatName", model.FormatId);
            ViewData["SiteId"] = new SelectList(_context.TblSites, "SiteId", "SiteName", model.SiteId);

            return PartialView("Create", model);
        }

        public async Task<IActionResult> EditModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Materials))
                return NotFound();

            if (id == null) return NotFound();

            var entity = await _context.TblMaterials.FindAsync(id);
            if (entity == null) return NotFound();

            var model = new MaterialViewModel
            {
                MaterialId = entity.MaterialId,
                MaterialCode = entity.MaterialCode,
                MaterialDescription = entity.MaterialDescription,
                CollectionId = entity.CollectionId,
                FinitureId = entity.FinitureId,
                FormatId = entity.FormatId,
                SiteId = entity.SiteId,
                MaterialImg = entity.MaterialImg,
                MaterialReceivedDate = entity.MaterialReceivedDate,
                MaterialStock = entity.MaterialStock,
                UserId = entity.UserId,
            };

            ViewData["CollectionId"] = new SelectList(
                _context.TblCollections.Where(c => c.CollectionActive == true),
                "CollectionId",
                "CollectionName",
                model.CollectionId);

            ViewData["FinitureId"] = new SelectList(
                _context.TblFinitures.Where(f => f.FinitureActive == true),
                "FinitureId",
                "FinitureName",
                model.FinitureId);

            ViewData["FormatId"] = new SelectList(
                _context.TblFormats.Where(fo => fo.FormatActive == true),
                "FormatId",
                "FormatName",
                model.FormatId);

            ViewData["SiteId"] = new SelectList(
                _context.TblSites.Where(s => s.SiteActive == true),
                "SiteId",
                "SiteName",
                model.SiteId);

            ViewData["UserId"] = new SelectList(
                _context.TblUsers.Where(u => u.UserActive == true),
                "UserId",
                "UserName",
                model.UserId);

            return PartialView("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, MaterialViewModel model)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.Materials))
                return Json(new { success = false, message = "No tienes permiso para editar materiales." });

            if (id != model.MaterialId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = await _context.TblMaterials.FindAsync(id);
                    if (entity == null) return NotFound();

                    if (model.MaterialFile != null && model.MaterialFile.Length > 0)
                    {
                        var uploads = Path.Combine(_environment.WebRootPath, "UploadedImages");
                        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                        var fileName = Path.GetFileName(model.MaterialFile.FileName);
                        var filePath = Path.Combine(uploads, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.MaterialFile.CopyToAsync(stream);
                        }

                        entity.MaterialImg = fileName;
                    }

                    entity.MaterialCode = model.MaterialCode;
                    entity.MaterialDescription = model.MaterialDescription;
                    entity.CollectionId = model.CollectionId;
                    entity.FinitureId = model.FinitureId;
                    entity.FormatId = model.FormatId;
                    entity.SiteId = model.SiteId;
                    entity.MaterialReceivedDate = model.MaterialReceivedDate;
                    entity.MaterialStock = model.MaterialStock;

                    _context.Update(entity);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Material actualizado correctamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            ViewData["CollectionId"] = new SelectList(_context.TblCollections, "CollectionId", "CollectionName", model.CollectionId);
            ViewData["FinitureId"] = new SelectList(_context.TblFinitures, "FinitureId", "FinitureName", model.FinitureId);
            ViewData["FormatId"] = new SelectList(_context.TblFormats, "FormatId", "FormatName", model.FormatId);
            ViewData["SiteId"] = new SelectList(_context.TblSites, "SiteId", "SiteName", model.SiteId);

            return PartialView("Edit", model);
        }

        public async Task<IActionResult> DeleteModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Materials))
                return NotFound();

            if (id == null) return NotFound();

            var entity = await _context.TblMaterials
                .Include(m => m.Collection)
                .Include(m => m.Finiture)
                .Include(m => m.Format)
                .Include(m => m.Site)
                .FirstOrDefaultAsync(m => m.MaterialId == id);

            if (entity == null) return NotFound();

            return PartialView("Delete", entity);
        }

        [HttpPost, ActionName("DeleteModal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.Materials))
                return Json(new { success = false, message = "No tienes permiso para eliminar materiales." });

            try
            {
                var entity = await _context.TblMaterials.FindAsync(id);
                if (entity != null)
                {
                    _context.TblMaterials.Remove(entity);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Material eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool TblMaterialExists(int id)
        {
            return _context.TblMaterials.Any(e => e.MaterialId == id);
        }
    }
}
