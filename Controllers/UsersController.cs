using InventorySys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var inventorySysContext = _context.TblUsers.Include(t => t.Role);
            return View(await inventorySysContext.ToListAsync());
        }

        private bool TblUserExists(int id)
        {
            return _context.TblUsers.Any(e => e.UserId == id);
        }

        // === METODOS MODALS ===

        // GET: Users/CreateModal
        public IActionResult CreateModal()
        {
            ViewBag.RoleId = new SelectList(_context.TblRoles.Where(r => r.RoleActive == true), "RoleId", "RoleName");
            return PartialView("Create", new TblUser());
        }

        // POST: Users/CreateModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal([Bind("UserId,UserName,UserEmail,UserEncryptedPassword,RoleId,UserActive")] TblUser tblUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblUser);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Usuario creado exitosamente." });
            }
            ViewBag.RoleId = new SelectList(_context.TblRoles.Where(r => r.RoleActive == true), "RoleId", "RoleName", tblUser.RoleId);
            return PartialView("Create", tblUser);
        }

        // GET: Users/DetailsModal/5
        public async Task<IActionResult> DetailsModal(int? id)
        {
            if (id == null) return NotFound();
            var tblUser = await _context.TblUsers
                .Include(t => t.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (tblUser == null) return NotFound();
            return PartialView("Details", tblUser);
        }

        // GET: Users/EditModal/5
        public async Task<IActionResult> EditModal(int? id)
        {
            if (id == null) return NotFound();
            var tblUser = await _context.TblUsers.FindAsync(id);
            if (tblUser == null) return NotFound();
            ViewBag.RoleId = new SelectList(_context.TblRoles.Where(r => r.RoleActive == true), "RoleId", "RoleName", tblUser.RoleId);
            return PartialView("Edit", tblUser);
        }

        // POST: Users/EditModal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, [Bind("UserId,UserName,UserEmail,UserEncryptedPassword,RoleId,UserActive")] TblUser tblUser)
        {
            if (id != tblUser.UserId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblUser);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Usuario actualizado exitosamente." });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblUserExists(tblUser.UserId))
                        return NotFound();
                    else
                        throw;
                }
            }
            ViewBag.RoleId = new SelectList(_context.TblRoles.Where(r => r.RoleActive == true), "RoleId", "RoleName", tblUser.RoleId);
            return PartialView("Edit", tblUser);
        }

        // GET: Users/DeleteModal/5
        public async Task<IActionResult> DeleteModal(int? id)
        {
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
            var tblUser = await _context.TblUsers.FindAsync(id);
            if (tblUser != null)
            {
                _context.TblUsers.Remove(tblUser);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Usuario eliminado exitosamente." });
            }
            return Json(new { success = false, message = "Usuario no encontrado." });
        }
    }
}
