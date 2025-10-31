using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventorySys.Models;

namespace InventorySys.Controllers
{
    public class DetailMovementsController : Controller
    {
        private readonly InventorySysContext _context;

        public DetailMovementsController(InventorySysContext context)
        {
            _context = context;
        }

        // GET: DetailMovements
        public async Task<IActionResult> Index()
        {
            var inventorySysContext = _context.TblDetailMovements.Include(t => t.MaterialTransaction);
            return View(await inventorySysContext.ToListAsync());
        }

        // GET: DetailMovements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblDetailMovement = await _context.TblDetailMovements
                .Include(t => t.MaterialTransaction)
                .FirstOrDefaultAsync(m => m.DetailMovId == id);
            if (tblDetailMovement == null)
            {
                return NotFound();
            }

            return View(tblDetailMovement);
        }

        // GET: DetailMovements/Create
        public IActionResult Create()
        {
            ViewData["MaterialTransactionId"] = new SelectList(_context.TblMaterialTransactions, "MaterialTransactionId", "MaterialTransactionId");
            return View();
        }

        // POST: DetailMovements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DetailMovId,MaterialTransactionId,DetInitBalance,DetCantEntry,DetCantExit,DetCurrentBalance")] TblDetailMovement tblDetailMovement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblDetailMovement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaterialTransactionId"] = new SelectList(_context.TblMaterialTransactions, "MaterialTransactionId", "MaterialTransactionId", tblDetailMovement.MaterialTransactionId);
            return View(tblDetailMovement);
        }

        // GET: DetailMovements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblDetailMovement = await _context.TblDetailMovements.FindAsync(id);
            if (tblDetailMovement == null)
            {
                return NotFound();
            }
            ViewData["MaterialTransactionId"] = new SelectList(_context.TblMaterialTransactions, "MaterialTransactionId", "MaterialTransactionId", tblDetailMovement.MaterialTransactionId);
            return View(tblDetailMovement);
        }

        // POST: DetailMovements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DetailMovId,MaterialTransactionId,DetInitBalance,DetCantEntry,DetCantExit,DetCurrentBalance")] TblDetailMovement tblDetailMovement)
        {
            if (id != tblDetailMovement.DetailMovId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblDetailMovement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblDetailMovementExists(tblDetailMovement.DetailMovId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaterialTransactionId"] = new SelectList(_context.TblMaterialTransactions, "MaterialTransactionId", "MaterialTransactionId", tblDetailMovement.MaterialTransactionId);
            return View(tblDetailMovement);
        }

        // GET: DetailMovements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblDetailMovement = await _context.TblDetailMovements
                .Include(t => t.MaterialTransaction)
                .FirstOrDefaultAsync(m => m.DetailMovId == id);
            if (tblDetailMovement == null)
            {
                return NotFound();
            }

            return View(tblDetailMovement);
        }

        // POST: DetailMovements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblDetailMovement = await _context.TblDetailMovements.FindAsync(id);
            if (tblDetailMovement != null)
            {
                _context.TblDetailMovements.Remove(tblDetailMovement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblDetailMovementExists(int id)
        {
            return _context.TblDetailMovements.Any(e => e.DetailMovId == id);
        }
    }
}
