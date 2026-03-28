using Microsoft.AspNetCore.Mvc;

namespace doanweb.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: Cart/Clear
        public IActionResult Clear()
        {
            // Cart is managed by JavaScript/localStorage
            return RedirectToAction("Index");
        }
    }
}
