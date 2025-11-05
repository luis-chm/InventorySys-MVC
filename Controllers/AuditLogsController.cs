using DocumentFormat.OpenXml.InkML;
using InventorySys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySys.Controllers
{
    public class AuditLogsController : Controller
    {
        private readonly InventorySysContext _context;
        public AuditLogsController(InventorySysContext context)
        {
            _context = context;
        }

        // GET: AuditLogs
        public async Task<IActionResult> Index()
        {
            var inventorySysContext = _context.TblAuditLogs.Include(t => t.User);
            return View(await inventorySysContext.ToListAsync());
        }
    }
}
