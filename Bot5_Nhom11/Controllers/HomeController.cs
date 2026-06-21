using System.Diagnostics;
using doanweb.Data;
using doanweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GymDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, GymDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OnlinePackages()
        {
            return View();
        }

        public async Task<IActionResult> Gyms()
        {
            var packages = await _dbContext.Packages
                .AsNoTracking()
                .Where(p => p.Status == "Active" && p.PackageType == "Offline")
                .OrderBy(p => p.Price)
                .ToListAsync();

            return View(packages);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
