using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PackageController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<PackageController> _logger;

        public PackageController(GymDbContext dbContext, ILogger<PackageController> logger)
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

        // Danh sách gói tập
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/package/index" });
            }

            var packages = await _dbContext.Packages.ToListAsync();
            return View(packages);
        }

        // Trang thêm gói tập
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }
            return View();
        }

        // Xử lý thêm gói tập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Package model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.CreatedDate = DateTime.Now;
                model.Status = model.Status ?? "Active";

                _dbContext.Packages.Add(model);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Package created: {model.PackageName}");
                TempData["SuccessMessage"] = "Gói tập đã được tạo thành công!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating package: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo gói tập");
                return View(model);
            }
        }

        // Trang chỉnh sửa gói tập
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var package = await _dbContext.Packages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }

        // Xử lý chỉnh sửa gói tập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Package model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            if (id != model.PackageId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var existingPackage = await _dbContext.Packages.FindAsync(id);
                if (existingPackage == null)
                {
                    return NotFound();
                }

                existingPackage.PackageName = model.PackageName;
                existingPackage.Price = model.Price;
                existingPackage.DurationDays = model.DurationDays;
                existingPackage.Description = model.Description;
                existingPackage.Status = model.Status;
                existingPackage.PackageType = model.PackageType ?? existingPackage.PackageType;
                existingPackage.Category = model.Category ?? existingPackage.Category;
                existingPackage.Features = model.Features ?? existingPackage.Features;

                _dbContext.Packages.Update(existingPackage);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Package updated: {model.PackageName}");
                TempData["SuccessMessage"] = "Gói tập đã được cập nhật thành công!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating package: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật gói tập");
                return View(model);
            }
        }

        // Xóa gói tập
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            if (!IsAdmin())
            {
                return Json(new { success = false, message = "Không có quyền" });
            }

            if (request == null || request.id <= 0)
            {
                return Json(new { success = false, message = "ID gói tập không hợp lệ" });
            }

            try
            {
                int id = request.id;
                _logger.LogInformation($"Delete request received for package ID: {id}");
                
                var package = await _dbContext.Packages
                    .Include(p => p.Subscriptions)
                    .FirstOrDefaultAsync(p => p.PackageId == id);
                
                if (package == null)
                {
                    _logger.LogWarning($"Package with ID {id} not found");
                    return Json(new { success = false, message = "Gói tập không tồn tại" });
                }

                _logger.LogInformation($"Found package: {package.PackageName}");

                // Delete related subscriptions first (if any)
                if (package.Subscriptions != null && package.Subscriptions.Any())
                {
                    _logger.LogInformation($"Deleting {package.Subscriptions.Count} subscriptions");
                    var subscriptionsToDelete = package.Subscriptions.ToList();
                    foreach (var subscription in subscriptionsToDelete)
                    {
                        _dbContext.Subscriptions.Remove(subscription);
                    }
                }

                _dbContext.Packages.Remove(package);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Package deleted successfully: {package.PackageName}");
                return Json(new { success = true, message = "Gói tập đã được xóa thành công!" });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Database error deleting package: {ex.Message}\n{ex.InnerException}");
                return Json(new { success = false, message = "Không thể xóa gói tập này vì có dữ liệu liên quan" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting package: {ex.Message}\n{ex.StackTrace}");
                return Json(new { success = false, message = $"Đã xảy ra lỗi khi xóa gói tập: {ex.Message}" });
            }
        }

        // Helper class for JSON body
        public class DeleteRequest
        {
            public int id { get; set; }
        }

        // Lấy chi tiết gói tập (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetPackageDetails(int id)
        {
            var package = await _dbContext.Packages.FindAsync(id);
            if (package == null)
            {
                return Json(new { success = false });
            }

            return Json(new { success = true, data = package });
        }
    }
}
