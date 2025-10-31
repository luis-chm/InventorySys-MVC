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

        //queda pendiente implementacion de dashboard controller

        public DashboardController(InventorySysContext context)
        {
            _context = context;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var dashboardData = new Dictionary<string, object>();

            // Total de Materiales
            dashboardData["TotalMateriales"] = await _context.TblMaterials.CountAsync();

            // Stock Total
            dashboardData["StockTotal"] = await _context.TblMaterials
                .SumAsync(m => (decimal?)m.MaterialStock) ?? 0;

            // Total de Transacciones
            dashboardData["TotalTransacciones"] = await _context.TblMaterialTransactions.CountAsync();

            // Colecciones Activas
            dashboardData["ColeccionesActivas"] = await _context.TblCollections
                .Where(c => c.CollectionActive == true)
                .CountAsync();

            // Sitios Activos
            dashboardData["SitiosActivos"] = await _context.TblSites
                .Where(s => s.SiteActive == true)
                .CountAsync();

            // Formatos Activos
            dashboardData["FormatosActivos"] = await _context.TblFormats
                .Where(f => f.FormatActive == true)
                .CountAsync();

            // Acabados Activos
            dashboardData["AcabadosActivos"] = await _context.TblFinitures
                .Where(f => f.FinitureActive == true)
                .CountAsync();

            // Usuarios Registrados
            dashboardData["UsuariosRegistrados"] = await _context.TblUsers.CountAsync();

            return View(dashboardData);
        }


    }
}
