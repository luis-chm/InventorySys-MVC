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
    public class CollectionsController : Controller
    {
        private readonly InventorySysContext _context;

        public CollectionsController(InventorySysContext context)
        {
            _context = context;
        }

        // GET: Collections
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblCollections.ToListAsync());
        }

        // GET: Collections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblCollection = await _context.TblCollections
                .FirstOrDefaultAsync(m => m.CollectionId == id);
            if (tblCollection == null)
            {
                return NotFound();
            }

            return View(tblCollection);
        }

        // GET: Collections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CollectionId,CollectionName,CollectionEffect,CollectionActive")] TblCollection tblCollection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblCollection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblCollection);
        }

        // GET: Collections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblCollection = await _context.TblCollections.FindAsync(id);
            if (tblCollection == null)
            {
                return NotFound();
            }
            return View(tblCollection);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CollectionId,CollectionName,CollectionEffect,CollectionActive")] TblCollection tblCollection)
        {
            if (id != tblCollection.CollectionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblCollection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblCollectionExists(tblCollection.CollectionId))
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
            return View(tblCollection);
        }

        // GET: Collections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblCollection = await _context.TblCollections
                .FirstOrDefaultAsync(m => m.CollectionId == id);
            if (tblCollection == null)
            {
                return NotFound();
            }

            return View(tblCollection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblCollection = await _context.TblCollections.FindAsync(id);
            if (tblCollection != null)
            {
                _context.TblCollections.Remove(tblCollection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblCollectionExists(int id)
        {
            return _context.TblCollections.Any(e => e.CollectionId == id);
        }
    }
}
