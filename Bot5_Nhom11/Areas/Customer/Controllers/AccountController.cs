using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;
using doanweb.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace doanweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("customer/account")]
    public class AccountController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly IOAuthService _oauthService;

        public AccountController(GymDbContext dbContext, ILogger<AccountController> logger, IOAuthService oauthService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _oauthService = oauthService;
        }

        // OAuth Login - Facebook
        [HttpGet("login-with-facebook")]
        public IActionResult LoginWithFacebook(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                HttpContext.Session.SetString("ReturnUrl", returnUrl);
            }

            // Manual OAuth flow - bypass ASP.NET Core authentication
            var appId = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()["Authentication:Facebook:AppId"];
            
            var redirectUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/customer/account/facebook-callback";
            var facebookUrl = $"https://www.facebook.com/v18.0/dialog/oauth?client_id={appId}&redirect_uri={System.Web.HttpUtility.UrlEncode(redirectUri)}&scope=public_profile&state={Guid.NewGuid()}";
            
            return Redirect(facebookUrl);
        }

        // OAuth Callback - Facebook
        [HttpGet("facebook-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> FacebookCallback(string code, string state)
        {
            try
            {
                _logger.LogInformation($"FacebookCallback: code={code}, state={state}");

                if (string.IsNullOrEmpty(code))
                {
                    _logger.LogWarning("No code from Facebook");
                    TempData["ErrorMessage"] = "Facebook login cancelled";
                    return RedirectToAction(nameof(Login));
                }

                // Exchange code for access token
                var appId = HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["Authentication:Facebook:AppId"];
                var appSecret = HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["Authentication:Facebook:AppSecret"];

                var redirectUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/customer/account/facebook-callback";
                var tokenUrl = $"https://graph.facebook.com/v18.0/oauth/access_token?client_id={appId}&client_secret={appSecret}&redirect_uri={System.Web.HttpUtility.UrlEncode(redirectUri)}&code={code}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(tokenUrl);
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Token response: {content}");

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Failed to get token: {content}");
                        TempData["ErrorMessage"] = "Failed to authenticate with Facebook";
                        return RedirectToAction(nameof(Login));
                    }

                    dynamic tokenData = System.Text.Json.JsonSerializer.Deserialize<dynamic>(content);
                    var accessToken = tokenData.GetProperty("access_token").GetString();

                    // Get user info
                    var userUrl = $"https://graph.facebook.com/v18.0/me?fields=id,name,picture&access_token={accessToken}";
                    response = await client.GetAsync(userUrl);
                    content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"User response: {content}");

                    dynamic userData = System.Text.Json.JsonSerializer.Deserialize<dynamic>(content);
                    var facebookId = userData.GetProperty("id").GetString();
                    var name = userData.GetProperty("name").GetString();

                    // Create or update user
                    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == $"fb_{facebookId}@facebook.com");
                    
                    if (user == null)
                    {
                        user = new User
                        {
                            FullName = name,
                            Email = $"fb_{facebookId}@facebook.com",
                            PhoneNumber = "0000000000",
                            PasswordHash = "facebook_login",
                            Status = "Active",
                            CreatedDate = DateTime.Now
                        };
                        _dbContext.Users.Add(user);
                        await _dbContext.SaveChangesAsync();
                    }

                    // Set session
                    var roleName = await GetUserRole(user.UserId);
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserName", user.FullName);
                    HttpContext.Session.SetString("UserRole", roleName);

                    _logger.LogInformation($"User logged in: {user.Email}");

                    if (roleName == "Admin")
                        return RedirectToAction("Index", "Home", new { area = "Admin" });

                    var returnUrl = HttpContext.Session.GetString("ReturnUrl");
                    HttpContext.Session.Remove("ReturnUrl");
                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"FacebookCallback error: {ex.Message}");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Login));
            }
        }

        // Trang ??ng ký
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        // X? lý ??ng ký
        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được đăng ký ký");
                    return View(model);
                }

                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khảu không khớp");
                    return View(model);
                }

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

                SetUserSession(user);

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi, vui lòng thử lại.");
                return View(model);
            }
        }

        // Trang ??ng nh?p
        [HttpGet("login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // X? lý ??ng nh?p
        [HttpPost("login")]
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

                SetUserSession(user);

                var roleName = await GetUserRole(user.UserId);
                HttpContext.Session.SetString("UserRole", roleName);

                _logger.LogInformation($"User logged in: {user.Email} with role: {roleName}");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                if (roleName == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging in user: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi ? vui lòng thử lại");
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }
        }

        // ??ng xu?t
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // Trang H? s? cá nhân
        [HttpGet("profile")]
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

        // X? lý c?p nh?t h? s? cá nhân
        [HttpPost("profile")]
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

                var emailCheck = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.UserId != userId.Value);
                
                if (emailCheck != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng");
                    return View(model);
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.DateOfBirth = model.DateOfBirth ?? DateTime.MinValue;
                user.Address = model.Address;
                user.Gender = model.Gender;
                user.UpdatedDate = DateTime.Now;

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserEmail", user.Email);

                TempData["SuccessMessage"] = "Cập nhật hồ sơ  thành công!";
                _logger.LogInformation($"User profile updated: {user.Email}");

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating profile: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi vui lòng thử lại.");
                return View(model);
            }
        }

        private void SetUserSession(User user)
        {
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);
        }

        private async Task<string> GetUserRole(int userId)
        {
            var userRole = await _dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync();
            
            return userRole?.Role?.RoleName ?? "Customer";
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash);
        }
    }
}
