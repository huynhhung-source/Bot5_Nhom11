using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(GymDbContext dbContext, ILogger<HomeController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Kiểm tra quyền Admin
        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(userRole) && userRole == "Admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/home/index" });
            }

            // Lấy thống kê
            var totalUsers = await _dbContext.Users.CountAsync();
            var totalPackages = await _dbContext.Packages.CountAsync();
            var totalSubscriptions = await _dbContext.Subscriptions.CountAsync();
            var totalRevenue = await _dbContext.Payments.Where(p => p.Status == "Success").SumAsync(p => p.Amount);
            var activeSubscriptions = await _dbContext.Subscriptions.Where(s => s.Status == "Active").CountAsync();
            var totalClasses = await _dbContext.Classes.CountAsync();
            var totalPayments = await _dbContext.Payments.CountAsync();
            var activeUsers = await _dbContext.Users.Where(u => u.Status == "Active").CountAsync();

            // Tạo ViewModel
            var viewModel = new DashboardViewModel
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalPackages = totalPackages,
                TotalSubscriptions = totalSubscriptions,
                ActiveSubscriptions = activeSubscriptions,
                TotalRevenue = totalRevenue,
                TotalClasses = totalClasses,
                TotalPayments = totalPayments
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/home/dashboard" });
            }

            var totalUsers = await _dbContext.Users.CountAsync();
            var totalPackages = await _dbContext.Packages.CountAsync();
            var totalSubscriptions = await _dbContext.Subscriptions.CountAsync();
            var totalRevenue = await _dbContext.Payments.Where(p => p.Status == "Success").SumAsync(p => p.Amount);

            var viewModel = new DashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalPackages = totalPackages,
                TotalSubscriptions = totalSubscriptions,
                TotalRevenue = totalRevenue
            };

            return View(viewModel);
        }

        public IActionResult Trainers()
        {
            return RedirectToAction("Trainers", "Home", new { area = "" });
        }

        public IActionResult TrainerDetail(int id)
        {
            return RedirectToAction("TrainerDetail", "Home", new { area = "", id });
        }
    }

    // ViewModel cho Dashboard
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalPackages { get; set; }
        public int TotalSubscriptions { get; set; }
        public int ActiveSubscriptions { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalClasses { get; set; }
        public int TotalPayments { get; set; }
    }
}
