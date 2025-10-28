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
    public class FinituresController : Controller
    {
        private readonly InventorySysContext _context;

        public FinituresController(InventorySysContext context)
        {
            _context = context;
        }

        // GET: Finitures
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblFinitures.ToListAsync());
        }

        // GET: Finitures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblFiniture = await _context.TblFinitures
                .FirstOrDefaultAsync(m => m.FinitureId == id);
            if (tblFiniture == null)
            {
                return NotFound();
            }

            return View(tblFiniture);
        }

        // GET: Finitures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Finitures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FinitureId,FinitureCode,FinitureName,FinitureActive")] TblFiniture tblFiniture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblFiniture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblFiniture);
        }

        // GET: Finitures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblFiniture = await _context.TblFinitures.FindAsync(id);
            if (tblFiniture == null)
            {
                return NotFound();
            }
            return View(tblFiniture);
        }

        // POST: Finitures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FinitureId,FinitureCode,FinitureName,FinitureActive")] TblFiniture tblFiniture)
        {
            if (id != tblFiniture.FinitureId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblFiniture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblFinitureExists(tblFiniture.FinitureId))
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
            return View(tblFiniture);
        }

        // GET: Finitures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblFiniture = await _context.TblFinitures
                .FirstOrDefaultAsync(m => m.FinitureId == id);
            if (tblFiniture == null)
            {
                return NotFound();
            }

            return View(tblFiniture);
        }

        // POST: Finitures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblFiniture = await _context.TblFinitures.FindAsync(id);
            if (tblFiniture != null)
            {
                _context.TblFinitures.Remove(tblFiniture);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblFinitureExists(int id)
        {
            return _context.TblFinitures.Any(e => e.FinitureId == id);
        }
    }
}
