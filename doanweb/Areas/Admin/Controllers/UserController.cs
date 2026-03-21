using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(GymDbContext dbContext, ILogger<UserController> logger)
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

        // Lấy danh sách role để hiển thị trong dropdown
        private async Task PopulateRoles()
        {
            var roles = await _dbContext.Roles
                .Where(r => r.Status == "Active")
                .ToListAsync();
            ViewData["Roles"] = roles;
        }

        // Danh sách người dùng
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/user/index" });
            }

            var users = await _dbContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();
            return View(users);
        }

        // Trang thêm người dùng
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }
            await PopulateRoles();
            return View();
        }

        // Xử lý thêm người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User model, int? roleId)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            // Kiểm tra email đã tồn tại
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký");
            }

            if (!ModelState.IsValid)
            {
                await PopulateRoles();
                return View(model);
            }

            try
            {
                model.CreatedDate = DateTime.Now;
                model.Status = "Active";
                // Hash password
                model.PasswordHash = HashPassword("User@123"); // Default password

                _dbContext.Users.Add(model);
                await _dbContext.SaveChangesAsync();

                // Assign role
                int selectedRoleId = roleId ?? await _dbContext.Roles
                    .Where(r => r.RoleName == "Customer" && r.Status == "Active")
                    .Select(r => r.RoleId)
                    .FirstOrDefaultAsync();

                if (selectedRoleId > 0)
                {
                    var userRole = new UserRole
                    {
                        UserId = model.UserId,
                        RoleId = selectedRoleId,
                        AssignedDate = DateTime.Now
                    };
                    _dbContext.UserRoles.Add(userRole);
                    await _dbContext.SaveChangesAsync();
                }

                _logger.LogInformation($"User created: {model.FullName}");
                TempData["SuccessMessage"] = "Người dùng đã được tạo thành công!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating user: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo người dùng");
                await PopulateRoles();
                return View(model);
            }
        }

        // Trang chỉnh sửa người dùng
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == id);
            
            if (user == null)
            {
                return NotFound();
            }

            await PopulateRoles();
            return View(user);
        }

        // Xử lý chỉnh sửa người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User model, int? roleId)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            if (id != model.UserId)
            {
                return BadRequest();
            }

            // Kiểm tra email trùng
            var emailCheck = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.UserId != id);
            if (emailCheck != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng");
            }

            if (!ModelState.IsValid)
            {
                model.UserRoles = await _dbContext.UserRoles.Where(ur => ur.UserId == id).ToListAsync();
                await PopulateRoles();
                return View(model);
            }

            try
            {
                var existingUser = await _dbContext.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.UserId == id);
                
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.FullName = model.FullName;
                existingUser.Email = model.Email;
                existingUser.PhoneNumber = model.PhoneNumber;
                existingUser.Address = model.Address;
                existingUser.DateOfBirth = model.DateOfBirth;
                existingUser.Gender = model.Gender;
                existingUser.Status = model.Status;
                // Only update password if a new one is provided
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    existingUser.PasswordHash = model.PasswordHash;
                }
                existingUser.UpdatedDate = DateTime.Now;

                _dbContext.Users.Update(existingUser);

                // Cập nhật role nếu được chỉ định
                if (roleId.HasValue && roleId.Value > 0)
                {
                    // Xóa role cũ
                    var oldUserRoles = existingUser.UserRoles.ToList();
                    foreach (var oldRole in oldUserRoles)
                    {
                        _dbContext.UserRoles.Remove(oldRole);
                    }

                    // Thêm role mới
                    var newUserRole = new UserRole
                    {
                        UserId = id,
                        RoleId = roleId.Value,
                        AssignedDate = DateTime.Now
                    };
                    _dbContext.UserRoles.Add(newUserRole);
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User updated: {model.FullName}");
                TempData["SuccessMessage"] = "Người dùng đã được cập nhật thành công!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật người dùng");
                model.UserRoles = await _dbContext.UserRoles.Where(ur => ur.UserId == id).ToListAsync();
                await PopulateRoles();
                return View(model);
            }
        }

        // Xóa người dùng
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            if (!IsAdmin())
            {
                return Json(new { success = false, message = "Không có quyền" });
            }

            if (request == null || request.id <= 0)
            {
                return Json(new { success = false, message = "ID người dùng không hợp lệ" });
            }

            try
            {
                int id = request.id;
                _logger.LogInformation($"Delete request received for user ID: {id}");

                var user = await _dbContext.Users
                    .Include(u => u.Subscriptions)
                    .Include(u => u.ClassEnrollments)
                    .Include(u => u.Payments)
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found");
                    return Json(new { success = false, message = "Người dùng không tồn tại" });
                }

                _logger.LogInformation($"Found user: {user.FullName}");

                // Delete related data
                if (user.Subscriptions != null && user.Subscriptions.Any())
                {
                    _logger.LogInformation($"Deleting {user.Subscriptions.Count} subscriptions");
                    foreach (var subscription in user.Subscriptions)
                    {
                        _dbContext.Subscriptions.Remove(subscription);
                    }
                }

                if (user.ClassEnrollments != null && user.ClassEnrollments.Any())
                {
                    _logger.LogInformation($"Deleting {user.ClassEnrollments.Count} class enrollments");
                    foreach (var enrollment in user.ClassEnrollments)
                    {
                        _dbContext.ClassEnrollments.Remove(enrollment);
                    }
                }

                if (user.Payments != null && user.Payments.Any())
                {
                    _logger.LogInformation($"Deleting {user.Payments.Count} payments");
                    foreach (var payment in user.Payments)
                    {
                        _dbContext.Payments.Remove(payment);
                    }
                }

                if (user.UserRoles != null && user.UserRoles.Any())
                {
                    _logger.LogInformation($"Deleting {user.UserRoles.Count} user roles");
                    foreach (var userRole in user.UserRoles)
                    {
                        _dbContext.UserRoles.Remove(userRole);
                    }
                }

                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User deleted successfully: {user.FullName}");
                return Json(new { success = true, message = "Người dùng đã được xóa thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user: {ex.Message}\n{ex.StackTrace}");
                return Json(new { success = false, message = $"Đã xảy ra lỗi khi xóa người dùng: {ex.Message}" });
            }
        }

        // Helper class for JSON body
        public class DeleteRequest
        {
            public int id { get; set; }
        }

        // Xem chi tiết người dùng
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var user = await _dbContext.Users
                .Include(u => u.Subscriptions)
                    .ThenInclude(s => s.Package)
                .Include(u => u.Payments)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Hash password helper
        private static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
