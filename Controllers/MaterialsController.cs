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
    public class MaterialsController : Controller
    {
        private readonly InventorySysContext _context;

        public MaterialsController(InventorySysContext context)
        {
            _context = context;
        }

        // GET: Materials
        public async Task<IActionResult> Index()
        {
            var inventorySysContext = _context.TblMaterials.Include(t => t.Collection).Include(t => t.Finiture).Include(t => t.Format).Include(t => t.Site).Include(t => t.User);
            return View(await inventorySysContext.ToListAsync());
        }

        // GET: Materials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblMaterial = await _context.TblMaterials
                .Include(t => t.Collection)
                .Include(t => t.Finiture)
                .Include(t => t.Format)
                .Include(t => t.Site)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.MaterialId == id);
            if (tblMaterial == null)
            {
                return NotFound();
            }

            return View(tblMaterial);
        }

        // GET: Materials/Create
        public IActionResult Create()
        {
            ViewData["CollectionId"] = new SelectList(_context.TblCollections, "CollectionId", "CollectionId");
            ViewData["FinitureId"] = new SelectList(_context.TblFinitures, "FinitureId", "FinitureId");
            ViewData["FormatId"] = new SelectList(_context.TblFormats, "FormatId", "FormatId");
            ViewData["SiteId"] = new SelectList(_context.TblSites, "SiteId", "SiteId");
            ViewData["UserId"] = new SelectList(_context.TblUsers, "UserId", "UserId");
            return View();
        }

        // POST: Materials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaterialId,MaterialCode,MaterialDescription,CollectionId,FinitureId,FormatId,SiteId,MaterialImg,MaterialReceivedDate,MaterialStock,UserId,RecordInsertDateTime")] TblMaterial tblMaterial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CollectionId"] = new SelectList(_context.TblCollections, "CollectionId", "CollectionId", tblMaterial.CollectionId);
            ViewData["FinitureId"] = new SelectList(_context.TblFinitures, "FinitureId", "FinitureId", tblMaterial.FinitureId);
            ViewData["FormatId"] = new SelectList(_context.TblFormats, "FormatId", "FormatId", tblMaterial.FormatId);
            ViewData["SiteId"] = new SelectList(_context.TblSites, "SiteId", "SiteId", tblMaterial.SiteId);
            ViewData["UserId"] = new SelectList(_context.TblUsers, "UserId", "UserId", tblMaterial.UserId);
            return View(tblMaterial);
        }

        // GET: Materials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblMaterial = await _context.TblMaterials.FindAsync(id);
            if (tblMaterial == null)
            {
                return NotFound();
            }
            ViewData["CollectionId"] = new SelectList(_context.TblCollections, "CollectionId", "CollectionId", tblMaterial.CollectionId);
            ViewData["FinitureId"] = new SelectList(_context.TblFinitures, "FinitureId", "FinitureId", tblMaterial.FinitureId);
            ViewData["FormatId"] = new SelectList(_context.TblFormats, "FormatId", "FormatId", tblMaterial.FormatId);
            ViewData["SiteId"] = new SelectList(_context.TblSites, "SiteId", "SiteId", tblMaterial.SiteId);
            ViewData["UserId"] = new SelectList(_context.TblUsers, "UserId", "UserId", tblMaterial.UserId);
            return View(tblMaterial);
        }

        // POST: Materials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaterialId,MaterialCode,MaterialDescription,CollectionId,FinitureId,FormatId,SiteId,MaterialImg,MaterialReceivedDate,MaterialStock,UserId,RecordInsertDateTime")] TblMaterial tblMaterial)
        {
            if (id != tblMaterial.MaterialId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblMaterialExists(tblMaterial.MaterialId))
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
            ViewData["CollectionId"] = new SelectList(_context.TblCollections, "CollectionId", "CollectionId", tblMaterial.CollectionId);
            ViewData["FinitureId"] = new SelectList(_context.TblFinitures, "FinitureId", "FinitureId", tblMaterial.FinitureId);
            ViewData["FormatId"] = new SelectList(_context.TblFormats, "FormatId", "FormatId", tblMaterial.FormatId);
            ViewData["SiteId"] = new SelectList(_context.TblSites, "SiteId", "SiteId", tblMaterial.SiteId);
            ViewData["UserId"] = new SelectList(_context.TblUsers, "UserId", "UserId", tblMaterial.UserId);
            return View(tblMaterial);
        }

        // GET: Materials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblMaterial = await _context.TblMaterials
                .Include(t => t.Collection)
                .Include(t => t.Finiture)
                .Include(t => t.Format)
                .Include(t => t.Site)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.MaterialId == id);
            if (tblMaterial == null)
            {
                return NotFound();
            }

            return View(tblMaterial);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblMaterial = await _context.TblMaterials.FindAsync(id);
            if (tblMaterial != null)
            {
                _context.TblMaterials.Remove(tblMaterial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblMaterialExists(int id)
        {
            return _context.TblMaterials.Any(e => e.MaterialId == id);
        }
    }
}
