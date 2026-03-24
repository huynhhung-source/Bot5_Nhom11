using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;
using System.Security.Cryptography;
using System.Text;

namespace doanweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;

        public AccountController(GymDbContext dbContext, ILogger<AccountController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Trang Đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Xử lý Đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra email đã tồn tại
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được đăng ký");
                    return View(model);
                }

                // Kiểm tra mật khẩu khớp
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu không khớp");
                    return View(model);
                }

                // Tạo user mới
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    PasswordHash = HashPassword(model.Password),
                    Address = "",
                    Gender = "Other",
                    CreatedDate = DateTime.Now,
                    Status = "Active"
                };

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User registered successfully: {user.Email}");

                // Đặt session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.FullName);
                // Mặc định role là Customer khi đăng ký
                HttpContext.Session.SetString("UserRole", "Customer");

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi. Vui lòng thử lại.");
                return View(model);
            }
        }

        // Trang Đăng nhập
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Xử lý Đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");
                    ViewData["ReturnUrl"] = returnUrl;
                    return View(model);
                }

                if (user.Status != "Active")
                {
                    ModelState.AddModelError("", "Tài khoản này đã bị khóa");
                    ViewData["ReturnUrl"] = returnUrl;
                    return View(model);
                }

                // Đặt session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.FullName);
                
                // Lấy role từ database
                var userRole = await _dbContext.UserRoles
                    .Where(ur => ur.UserId == user.UserId)
                    .Include(ur => ur.Role)
                    .FirstOrDefaultAsync();
                
                var roleName = userRole?.Role?.RoleName ?? "Customer";
                HttpContext.Session.SetString("UserRole", roleName);

                _logger.LogInformation($"User logged in: {user.Email} with role: {roleName}");

                // Kiểm tra nếu có returnUrl từ query string - ưu tiên returnUrl
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Nếu là Admin và không có returnUrl, redirect sang Admin dashboard
                if (roleName == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging in user: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi. Vui lòng thử lại.");
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }
        }

        // Đăng xuất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // Trang Hồ sơ cá nhân
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = await _dbContext.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Gender = user.Gender
            };

            return View(model);
        }

        // Xử lý cập nhật hồ sơ cá nhân
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _dbContext.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    return NotFound();
                }

                // Kiểm tra email trùng (ngoài user hiện tại)
                var emailCheck = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.UserId != userId.Value);
                
                if (emailCheck != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng");
                    return View(model);
                }

                // Cập nhật thông tin
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.DateOfBirth = model.DateOfBirth ?? DateTime.MinValue;
                user.Address = model.Address;
                user.Gender = model.Gender;
                user.UpdatedDate = DateTime.Now;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                // Cập nhật session
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserEmail", user.Email);

                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                _logger.LogInformation($"User profile updated: {user.Email}");

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating profile: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi. Vui lòng thử lại.");
                return View(model);
            }
        }

        // Hàm hash mật khẩu
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Hàm xác minh mật khẩu
        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash);
        }
    }
}
