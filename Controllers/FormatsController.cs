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
    public class FormatsController : Controller
    {
        private readonly InventorySysContext _context;

        public FormatsController(InventorySysContext context)
        {
            _context = context;
        }

        // GET: Formats
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblFormats.ToListAsync());
        }

        // GET: Formats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblFormat = await _context.TblFormats
                .FirstOrDefaultAsync(m => m.FormatId == id);
            if (tblFormat == null)
            {
                return NotFound();
            }

            return View(tblFormat);
        }

        // GET: Formats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Formats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormatId,FormatName,FormatSizeCm,FormatActive")] TblFormat tblFormat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblFormat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblFormat);
        }

        // GET: Formats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblFormat = await _context.TblFormats.FindAsync(id);
            if (tblFormat == null)
            {
                return NotFound();
            }
            return View(tblFormat);
        }

        // POST: Formats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FormatId,FormatName,FormatSizeCm,FormatActive")] TblFormat tblFormat)
        {
            if (id != tblFormat.FormatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblFormat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblFormatExists(tblFormat.FormatId))
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
            return View(tblFormat);
        }

        // GET: Formats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblFormat = await _context.TblFormats
                .FirstOrDefaultAsync(m => m.FormatId == id);
            if (tblFormat == null)
            {
                return NotFound();
            }

            return View(tblFormat);
        }

        // POST: Formats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblFormat = await _context.TblFormats.FindAsync(id);
            if (tblFormat != null)
            {
                _context.TblFormats.Remove(tblFormat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblFormatExists(int id)
        {
            return _context.TblFormats.Any(e => e.FormatId == id);
        }
    }
}
