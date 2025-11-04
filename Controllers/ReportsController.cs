using InventorySys.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventorySys.Controllers
{
    public class ReportsController : Controller
    {
        private readonly InventorySysContext _context;

        public ReportsController(InventorySysContext context)
        {
            _context = context;
        }

        public IActionResult ReportsMaterials()
        {
            return View();
        }

        public IActionResult ReportsTransactions()
        {
            return View();
        }
    }
}
