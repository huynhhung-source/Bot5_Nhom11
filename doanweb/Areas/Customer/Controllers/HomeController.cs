using Microsoft.AspNetCore.Mvc;

namespace doanweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Kiểm tra xem user có đăng nhập không
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/customer/home/index" });
            }
            return View();
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/customer/home/profile" });
            }
            return View();
        }
    }
}
