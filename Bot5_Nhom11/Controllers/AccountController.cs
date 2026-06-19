using Microsoft.AspNetCore.Mvc;

namespace doanweb.Controllers
{
    public class AccountController : Controller
    {
        // Redirect t?t c? request t? /Account/* sang /customer/account/*
        
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = returnUrl });
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string returnUrl = null)
        {
            return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = returnUrl });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return RedirectToAction("Register", "Account", new { area = "Customer" });
        }

        [HttpPost]
        public IActionResult Register(string fullName, string email, string phone, string password)
        {
            return RedirectToAction("Register", "Account", new { area = "Customer" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "Account", new { area = "Customer" });
        }
    }
}
