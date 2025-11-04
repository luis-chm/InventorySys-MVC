using Microsoft.AspNetCore.Mvc;

namespace InventorySys.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
