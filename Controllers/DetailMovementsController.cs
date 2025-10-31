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
            var detailMovements = _context.TblDetailMovements
                .Include(d => d.MaterialTransaction)
                .ThenInclude(m => m.Material)
                .ToListAsync();

            return View(await detailMovements);
        }
        private bool TblDetailMovementExists(int id)
        {
            return _context.TblDetailMovements.Any(e => e.DetailMovId == id);
        }
    }

}
