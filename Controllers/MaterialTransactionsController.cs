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
    public class MaterialTransactionsController : Controller
    {
        private readonly InventorySysContext _context;

        public MaterialTransactionsController(InventorySysContext context)
        {
            _context = context;
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        // GET: MaterialTransactions
        public async Task<IActionResult> Index()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanAccessModule(userRole, RolePermissionHelper.SystemModule.MaterialTransactions))
                return RedirectToAction("AccessDenied", "Home");

            var materialTransactions = _context.TblMaterialTransactions
                .Include(mt => mt.Material)
                .Include(mt => mt.User);

            return View(await materialTransactions.ToListAsync());
        }

        // GET: MaterialTransactions/CreateModal
        public IActionResult CreateModal()
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.MaterialTransactions))
                return NotFound();

            ViewData["MaterialId"] = new SelectList(_context.TblMaterials, "MaterialId", "MaterialCode");
            return PartialView("Create", new TblMaterialTransaction());
        }

        // POST: MaterialTransactions/CreateModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(TblMaterialTransaction tblMaterialTransaction)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanCreate(userRole, RolePermissionHelper.SystemModule.MaterialTransactions))
                return Json(new { success = false, message = "No tienes permiso para crear transacciones." });

            var currentUserName = User?.Identity?.Name;
            var currentUser = _context.TblUsers.FirstOrDefault(u => u.UserName == currentUserName);
            tblMaterialTransaction.UserId = currentUser?.UserId ?? 0;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(tblMaterialTransaction);
                    await _context.SaveChangesAsync();

                    var material = await _context.TblMaterials.FindAsync(tblMaterialTransaction.MaterialId);
                    if (material != null)
                    {
                        decimal initialBalance = material.MaterialStock;
                        decimal cantEntry = tblMaterialTransaction.MaterialTransactionType == "Ingreso"
                            ? tblMaterialTransaction.MaterialTransactionQuantity
                            : 0;
                        decimal cantExit = tblMaterialTransaction.MaterialTransactionType == "Retiro"
                            ? tblMaterialTransaction.MaterialTransactionQuantity
                            : 0;
                        decimal currentBalance = initialBalance + cantEntry - cantExit;

                        material.MaterialStock = currentBalance;

                        var detailMovement = new TblDetailMovement
                        {
                            MaterialTransactionId = tblMaterialTransaction.MaterialTransactionId,
                            DetInitBalance = initialBalance,
                            DetCantEntry = cantEntry,
                            DetCantExit = cantExit,
                            DetCurrentBalance = currentBalance
                        };

                        _context.Add(detailMovement);
                        await _context.SaveChangesAsync();
                    }

                    return Json(new { success = true, message = "Transacción creada correctamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            ViewData["MaterialId"] = new SelectList(_context.TblMaterials, "MaterialId", "MaterialCode", tblMaterialTransaction.MaterialId);
            return PartialView("Create", tblMaterialTransaction);
        }

        // GET: MaterialTransactions/EditModal/5
        public async Task<IActionResult> EditModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.MaterialTransactions))
                return NotFound();

            if (id == null) return NotFound();

            var tblMaterialTransaction = await _context.TblMaterialTransactions.FindAsync(id);
            if (tblMaterialTransaction == null) return NotFound();

            // Actualizar fecha a la actual
            tblMaterialTransaction.MaterialTransactionDate = DateTime.Now;

            ViewData["MaterialId"] = new SelectList(_context.TblMaterials, "MaterialId", "MaterialCode", tblMaterialTransaction.MaterialId);

            return PartialView("Edit", tblMaterialTransaction);
        }

        // POST: MaterialTransactions/EditModal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, TblMaterialTransaction tblMaterialTransaction)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanEdit(userRole, RolePermissionHelper.SystemModule.MaterialTransactions))
                return Json(new { success = false, message = "No tienes permiso para editar transacciones." });

            if (id != tblMaterialTransaction.MaterialTransactionId) return NotFound();

            var currentUserName = User?.Identity?.Name;
            var currentUser = _context.TblUsers.FirstOrDefault(u => u.UserName == currentUserName);
            tblMaterialTransaction.UserId = currentUser?.UserId ?? 0;

            if (ModelState.IsValid)
            {
                try
                {
                    var originalTransaction = await _context.TblMaterialTransactions.FindAsync(id);
                    if (originalTransaction == null) return NotFound();

                    var oldDetailMovement = await _context.TblDetailMovements
                        .FirstOrDefaultAsync(d => d.MaterialTransactionId == id);
                    if (oldDetailMovement == null) return NotFound();

                    var material = await _context.TblMaterials.FindAsync(tblMaterialTransaction.MaterialId);
                    if (material == null) return NotFound();

                    // PASO 1: Revertir la transacción anterior
                    if (originalTransaction.MaterialTransactionType == "Ingreso")
                    {
                        material.MaterialStock -= originalTransaction.MaterialTransactionQuantity;
                    }
                    else if (originalTransaction.MaterialTransactionType == "Retiro")
                    {
                        material.MaterialStock += originalTransaction.MaterialTransactionQuantity;
                    }

                    // PASO 2: El balance inicial es el stock después de revertir
                    decimal initialBalance = material.MaterialStock;

                    // PASO 3: Aplicar la nueva transacción
                    decimal cantEntry = tblMaterialTransaction.MaterialTransactionType == "Ingreso"
                        ? tblMaterialTransaction.MaterialTransactionQuantity
                        : 0;
                    decimal cantExit = tblMaterialTransaction.MaterialTransactionType == "Retiro"
                        ? tblMaterialTransaction.MaterialTransactionQuantity
                        : 0;
                    decimal currentBalance = initialBalance + cantEntry - cantExit;

                    // Actualizar stock del material
                    material.MaterialStock = currentBalance;

                    // Actualizar transacción
                    originalTransaction.MaterialTransactionType = tblMaterialTransaction.MaterialTransactionType;
                    originalTransaction.MaterialTransactionQuantity = tblMaterialTransaction.MaterialTransactionQuantity;
                    originalTransaction.MaterialTransactionDate = tblMaterialTransaction.MaterialTransactionDate;
                    originalTransaction.MaterialId = tblMaterialTransaction.MaterialId;
                    originalTransaction.UserId = tblMaterialTransaction.UserId;

                    // Actualizar DetailMovement
                    oldDetailMovement.DetInitBalance = initialBalance;
                    oldDetailMovement.DetCantEntry = cantEntry;
                    oldDetailMovement.DetCantExit = cantExit;
                    oldDetailMovement.DetCurrentBalance = currentBalance;

                    _context.Update(originalTransaction);
                    _context.Update(oldDetailMovement);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Transacción actualizada correctamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            ViewData["MaterialId"] = new SelectList(_context.TblMaterials, "MaterialId", "MaterialCode", tblMaterialTransaction.MaterialId);
            return PartialView("Edit", tblMaterialTransaction);
        }

        // GET: MaterialTransactions/DeleteModal/5
        public async Task<IActionResult> DeleteModal(int? id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.MaterialTransactions))
                return NotFound();

            if (id == null) return NotFound();

            var tblMaterialTransaction = await _context.TblMaterialTransactions
                .Include(t => t.Material)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.MaterialTransactionId == id);

            if (tblMaterialTransaction == null) return NotFound();

            return PartialView("Delete", tblMaterialTransaction);
        }

        // POST: MaterialTransactions/DeleteModal/5
        [HttpPost, ActionName("DeleteModal")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedModal(int id)
        {
            var userRole = GetUserRole();

            if (!RolePermissionHelper.CanDelete(userRole, RolePermissionHelper.SystemModule.MaterialTransactions))
                return Json(new { success = false, message = "No tienes permiso para eliminar transacciones." });

            try
            {
                var tblMaterialTransaction = await _context.TblMaterialTransactions.FindAsync(id);
                if (tblMaterialTransaction != null)
                {
                    _context.TblMaterialTransactions.Remove(tblMaterialTransaction);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = "Transacción eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private bool TblMaterialTransactionExists(int id)
        {
            return _context.TblMaterialTransactions.Any(e => e.MaterialTransactionId == id);
        }
    }
}
