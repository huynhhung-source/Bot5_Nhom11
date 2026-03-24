using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;

namespace doanweb.Controllers
{
    public class TestPaymentController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<TestPaymentController> _logger;

        public TestPaymentController(GymDbContext dbContext, ILogger<TestPaymentController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Test endpoint ?? debug l?i thanh toán
        /// URL: /TestPayment/Debug
        /// </summary>
        [HttpGet]
        public IActionResult Debug()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var userName = HttpContext.Session.GetString("UserName");

                var debugInfo = new
                {
                    userId = userId,
                    userEmail = userEmail,
                    userName = userName,
                    isLoggedIn = userId.HasValue && userId.Value > 0,
                    timestamp = DateTime.Now,
                    requestPath = HttpContext.Request.Path,
                    queryString = HttpContext.Request.QueryString.ToString()
                };

                _logger.LogInformation($"[TestPayment Debug] {System.Text.Json.JsonSerializer.Serialize(debugInfo)}");

                return Json(debugInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[TestPayment Debug] Error: {ex.Message}", ex);
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint ?? ki?m tra database
        /// URL: /TestPayment/CheckDatabase
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                var packageCount = await _dbContext.Packages.CountAsync();
                var userCount = await _dbContext.Users.CountAsync();
                var paymentCount = await _dbContext.Payments.CountAsync();
                var subscriptionCount = await _dbContext.Subscriptions.CountAsync();

                var packages = await _dbContext.Packages.Where(p => p.Status == "Active").ToListAsync();

                _logger.LogInformation($"[TestPayment DB] Packages: {packageCount}, Users: {userCount}, Payments: {paymentCount}, Subscriptions: {subscriptionCount}");

                return Json(new
                {
                    database = "Connected",
                    packages = packageCount,
                    users = userCount,
                    payments = paymentCount,
                    subscriptions = subscriptionCount,
                    activePackages = packages.Select(p => new { p.PackageId, p.PackageName, p.Price, p.Status })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[TestPayment DB] Error: {ex.Message}", ex);
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint ?? ki?m tra current user
        /// URL: /TestPayment/CheckUser
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckUser()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                
                if (!userId.HasValue)
                {
                    return Json(new { error = "User not logged in", isLoggedIn = false });
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);
                
                if (user == null)
                {
                    return Json(new { error = $"User not found: {userId.Value}" });
                }

                var subscriptions = await _dbContext.Subscriptions
                    .Include(s => s.Package)
                    .Where(s => s.UserId == userId.Value)
                    .ToListAsync();

                var payments = await _dbContext.Payments
                    .Where(p => p.UserId == userId.Value)
                    .ToListAsync();

                _logger.LogInformation($"[TestPayment User] userId={userId.Value}, email={user.Email}, subscriptions={subscriptions.Count}, payments={payments.Count}");

                return Json(new
                {
                    user = new { user.UserId, user.Email, user.FullName, user.Status },
                    subscriptions = subscriptions.Select(s => new { s.SubscriptionId, s.PackageId, s.Package.PackageName, s.Status, s.StartDate, s.EndDate }),
                    payments = payments.Select(p => new { p.PaymentId, p.Amount, p.PaymentMethod, p.Status, p.PaymentDate })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[TestPayment User] Error: {ex.Message}", ex);
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint ?? ki?m tra package c? th?
        /// URL: /TestPayment/CheckPackage?packageId=1
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckPackage(int packageId)
        {
            try
            {
                var package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.PackageId == packageId);
                
                if (package == null)
                {
                    return Json(new { error = $"Package not found: {packageId}" });
                }

                var subscriptions = await _dbContext.Subscriptions
                    .Where(s => s.PackageId == packageId)
                    .CountAsync();

                _logger.LogInformation($"[TestPayment Package] packageId={packageId}, name={package.PackageName}, subscribers={subscriptions}");

                return Json(new
                {
                    package = package,
                    subscribers = subscriptions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[TestPayment Package] Error: {ex.Message}", ex);
                return Json(new { error = ex.Message });
            }
        }
    }
}
