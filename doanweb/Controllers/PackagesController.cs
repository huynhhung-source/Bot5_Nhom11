using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using doanweb.Models;
using doanweb.Data;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Controllers
{
    public class PackagesController : Controller
    {
        private readonly ILogger<PackagesController> _logger;
        private readonly GymDbContext _dbContext;

        public PackagesController(ILogger<PackagesController> logger, GymDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // Gói t?p online
        public async Task<IActionResult> Online()
        {
            var packages = await _dbContext.Packages
                .Where(p => p.Status == "Active")
                .ToListAsync();
            return View(packages);
        }

        // Gói t?p offline
        public async Task<IActionResult> Offline()
        {
            var packages = await _dbContext.Packages
                .Where(p => p.Status == "Active")
                .ToListAsync();
            return View(packages);
        }

        // Chi ti?t gói t?p
        public async Task<IActionResult> Detail(int id)
        {
            var package = await _dbContext.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            return View(package);
        }

        // ??ng ký gói t?p - ki?m tra ??ng nh?p
        public IActionResult Register(int packageId)
        {
            // Ki?m tra xem user ?ã ??ng nh?p ch?a
            var userId = HttpContext.Session.GetInt32("UserId");
            
            if (!userId.HasValue)
            {
                // L?u thông tin gói t?p vào Session ?? quay l?i sau ??ng nh?p
                HttpContext.Session.SetInt32("PackageId", packageId);
                
                // Chuy?n h??ng ??n trang Login
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = $"/packages/detail/{packageId}" });
            }

            // N?u ?ã ??ng nh?p, chuy?n h??ng ??n trang chi ti?t
            return RedirectToAction("Detail", new { id = packageId });
        }
    }
}
