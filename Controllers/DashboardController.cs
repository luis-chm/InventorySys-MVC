using InventorySys.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventorySys.Controllers
{
    public class DashboardController : Controller
    {
        private readonly InventorySysContext _context;

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

        // GET: Dashboard/GetCriticalStock

        [HttpGet]

        public async Task<IActionResult> GetCriticalStock()

        {

            try

            {

                var criticalStock = await _context.TblMaterials

                    .Include(m => m.Collection)

                    .Where(m => m.MaterialStock < 75)

                    .OrderBy(m => m.MaterialStock)

                    .Select(m => new

                    {

                        m.MaterialId,

                        m.MaterialCode,

                        m.MaterialDescription,

                        m.MaterialStock,

                        m.Collection.CollectionName,

                        StockStatus = m.MaterialStock == 0 ? "SIN STOCK" :

                                     m.MaterialStock < 50 ? "CRÍTICO" : "BAJO"

                    })

                    .ToListAsync();

                return Json(new { success = true, data = criticalStock });

            }

            catch (Exception ex)

            {

                return Json(new { success = false, message = ex.Message });

            }

        }

        // GET: Dashboard/GetRecentTransactions

        [HttpGet]

        public async Task<IActionResult> GetRecentTransactions(int take = 10)

        {

            try

            {

                var transactions = await _context.TblMaterialTransactions

                    .Include(m => m.Material)

                    .Include(m => m.User)

                    .OrderByDescending(m => m.MaterialTransactionDate)

                    .Take(take)

                    .Select(m => new

                    {

                        m.MaterialTransactionId,

                        m.Material.MaterialCode,

                        m.Material.MaterialDescription,

                        m.MaterialTransactionType,

                        m.MaterialTransactionQuantity,

                        FechaTransaccion = m.MaterialTransactionDate.ToString("dd/MM/yyyy HH:mm"),

                        m.User.UserName

                    })

                    .ToListAsync();

                return Json(new { success = true, data = transactions });

            }

            catch (Exception ex)

            {

                return Json(new { success = false, message = ex.Message });

            }

        }

    }
}
