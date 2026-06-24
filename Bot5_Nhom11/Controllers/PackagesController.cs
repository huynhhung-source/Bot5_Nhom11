using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;

namespace doanweb.Controllers
{
    public class PackagesController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<PackagesController> _logger;

        public PackagesController(GymDbContext dbContext, ILogger<PackagesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Trang Gói T?p Online
        [HttpGet]
        public async Task<IActionResult> Online()
        {
            try
            {
                _logger.LogInformation("[Packages Online] Loading online packages");
                
                var packages = await _dbContext.Packages
                    .Where(p => p.Status == "Active" && (p.PackageType == "Online" || p.PackageType == null))
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();

                _logger.LogInformation($"[Packages Online] Found {packages.Count} online packages");
                return View(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Packages Online] Error: {ex.Message}\n{ex.StackTrace}", ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải gói tập online";
                return RedirectToAction("Index", "Home");
            }
        }

        // Trang Gói T?p Offline
        [HttpGet]
        public async Task<IActionResult> Offline()
        {
            try
            {
                _logger.LogInformation("[Packages Offline] Loading offline packages");
                
                var packages = await _dbContext.Packages
                    .Where(p => p.Status == "Active" && p.PackageType == "Offline")
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();

                _logger.LogInformation($"[Packages Offline] Found {packages.Count} offline packages");
                return View(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Packages Offline] Error: {ex.Message}\n{ex.StackTrace}", ex);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải gói tập offline";
                return RedirectToAction("Index", "Home");
            }
        }

        // Chi ti?t gói t?p
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                _logger.LogInformation($"[Packages Detail] Loading package: id={id}");

                var package = await _dbContext.Packages
                    .FirstOrDefaultAsync(p => p.PackageId == id && p.Status == "Active");

                if (package == null)
                {
                    _logger.LogWarning($"[Packages Detail] Package not found: id={id}");
                    TempData["ErrorMessage"] = "Gói tập không tồn lại";
                    return RedirectToAction("Online");
                }

                _logger.LogInformation($"[Packages Detail] Package found: {package.PackageName}");
                return View(package);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Packages Detail] Error: {ex.Message}\n{ex.StackTrace}", ex);
                TempData["ErrorMessage"] = "Có lỗi xả ra khi tải thông tin gói tập";
                return RedirectToAction("Online");
            }
        }

        // API: L?y danh sách gói t?p theo lo?i (JSON)
        [HttpGet]
        public async Task<IActionResult> GetPackagesByType(string type)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    return BadRequest(new { success = false, message = "Vui lòng chọn loại gói tập" });
                }

                var packages = await _dbContext.Packages
                    .Where(p => p.Status == "Active" && 
                           (p.PackageType == type || (type == "Online" && p.PackageType == null)))
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();

                return Json(new { success = true, data = packages });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Packages GetPackagesByType] Error: {ex.Message}", ex);
                return Json(new { success = false, message = "Lỗi khi lấy dữ liệu" });
            }
        }

        // API: L?y chi ti?t gói t?p (JSON)
        [HttpGet]
        public async Task<IActionResult> GetPackageDetail(int id)
        {
            try
            {
                var package = await _dbContext.Packages
                    .FirstOrDefaultAsync(p => p.PackageId == id && p.Status == "Active");

                if (package == null)
                {
                    return Json(new { success = false, message = "Gói tập không tồn tại" });
                }

                return Json(new { success = true, data = package });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Packages GetPackageDetail] Error: {ex.Message}", ex);
                return Json(new { success = false, message = "Lỗi khi lấy dữ liệu" });
            }
        }
    }
}
