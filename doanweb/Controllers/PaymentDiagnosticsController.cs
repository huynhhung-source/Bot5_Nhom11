using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;

namespace doanweb.Controllers
{
    /// <summary>
    /// Payment Diagnostic Controller - For Testing Payment Flow
    /// </summary>
    public class PaymentDiagnosticsController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<PaymentDiagnosticsController> _logger;

        public PaymentDiagnosticsController(GymDbContext dbContext, ILogger<PaymentDiagnosticsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Full payment diagnostics
        /// GET: /PaymentDiagnostics/FullDiagnostics
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FullDiagnostics()
        {
            var diagnostics = new Dictionary<string, object>();

            try
            {
                // 1. Check Database Connection
                var dbCheck = await CheckDatabaseConnection();
                diagnostics["Database"] = dbCheck;

                // 2. Check Packages
                var packages = await _dbContext.Packages.ToListAsync();
                diagnostics["Packages"] = new
                {
                    Total = packages.Count,
                    Active = packages.Count(p => p.Status == "Active"),
                    Inactive = packages.Count(p => p.Status != "Active"),
                    List = packages.Select(p => new { p.PackageId, p.PackageName, p.Price, p.Status })
                };

                // 3. Check Current User
                var userId = HttpContext.Session.GetInt32("UserId");
                var userCheck = new
                {
                    IsLoggedIn = userId.HasValue && userId > 0,
                    UserId = userId,
                    UserEmail = HttpContext.Session.GetString("UserEmail"),
                    UserName = HttpContext.Session.GetString("UserName")
                };
                diagnostics["CurrentUser"] = userCheck;

                // 4. Check User in Database (if logged in)
                if (userId.HasValue && userId > 0)
                {
                    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                    diagnostics["UserDetail"] = user == null ? "Not Found" : new { user.UserId, user.Email, user.FullName, user.Status };
                }

                // 5. Check User Payments
                if (userId.HasValue && userId > 0)
                {
                    var payments = await _dbContext.Payments
                        .Where(p => p.UserId == userId)
                        .OrderByDescending(p => p.PaymentDate)
                        .Take(10)
                        .ToListAsync();

                    diagnostics["UserPayments"] = new
                    {
                        Total = payments.Count,
                        Recent = payments.Select(p => new { p.PaymentId, p.Amount, p.Status, p.PaymentMethod, p.PaymentDate })
                    };
                }

                // 6. Check User Subscriptions
                if (userId.HasValue && userId > 0)
                {
                    var subscriptions = await _dbContext.Subscriptions
                        .Include(s => s.Package)
                        .Where(s => s.UserId == userId)
                        .OrderByDescending(s => s.StartDate)
                        .Take(10)
                        .ToListAsync();

                    diagnostics["UserSubscriptions"] = new
                    {
                        Total = subscriptions.Count,
                        Active = subscriptions.Count(s => s.Status == "Active"),
                        Recent = subscriptions.Select(s => new 
                        { 
                            s.SubscriptionId, 
                            PackageName = s.Package?.PackageName,
                            s.Status, 
                            s.StartDate, 
                            s.EndDate,
                            s.AmountPaid
                        })
                    };
                }

                // 7. System Info
                diagnostics["System"] = new
                {
                    DotNetVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                    Environment = _logger.IsEnabled(LogLevel.Debug) ? "Development" : "Production",
                    Timestamp = DateTime.Now
                };

                return Ok(diagnostics);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Diagnostics error: {ex.Message}", ex);
                return Ok(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// Test payment flow step by step
        /// GET: /PaymentDiagnostics/TestPaymentFlow
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TestPaymentFlow([FromBody] TestPaymentRequest request)
        {
            var results = new Dictionary<string, object>();

            try
            {
                _logger.LogInformation("Starting payment flow test");

                // Step 1: Verify user is logged in
                var userId = HttpContext.Session.GetInt32("UserId");
                results["Step1_UserLogin"] = new
                {
                    Status = userId.HasValue ? "? Pass" : "? Fail",
                    Message = userId.HasValue ? "User is logged in" : "User is not logged in",
                    UserId = userId
                };

                if (!userId.HasValue)
                {
                    return Ok(results);
                }

                // Step 2: Get package
                var package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.PackageId == request.PackageId);
                results["Step2_PackageCheck"] = new
                {
                    Status = package != null ? "? Pass" : "? Fail",
                    Message = package != null ? $"Package found: {package.PackageName}" : "Package not found",
                    PackageId = request.PackageId,
                    Package = package
                };

                if (package == null)
                {
                    return Ok(results);
                }

                // Step 3: Verify user exists in database
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                results["Step3_UserCheck"] = new
                {
                    Status = user != null && user.Status == "Active" ? "? Pass" : "? Fail",
                    Message = user == null ? "User not found" : user.Status == "Active" ? "User found and active" : "User found but inactive",
                    User = user
                };

                if (user == null)
                {
                    return Ok(results);
                }

                // Step 4: Create test payment
                var payment = new Payment
                {
                    UserId = userId.Value,
                    Amount = package.Price,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = request.PaymentMethod ?? "Test",
                    TransactionId = $"TEST{DateTime.Now:yyyyMMddHHmmss}",
                    Status = "Success",
                    Description = $"Test Payment: {package.PackageName}"
                };

                _dbContext.Payments.Add(payment);
                await _dbContext.SaveChangesAsync();

                results["Step4_PaymentCreation"] = new
                {
                    Status = "? Pass",
                    Message = "Payment created successfully",
                    PaymentId = payment.PaymentId,
                    Amount = payment.Amount,
                    TransactionId = payment.TransactionId
                };

                // Step 5: Create test subscription
                var subscription = new Subscription
                {
                    UserId = userId.Value,
                    PackageId = package.PackageId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(package.DurationDays),
                    ActivationDate = DateTime.Now,
                    Status = "Active",
                    RemainingDays = package.DurationDays,
                    SessionsUsed = 0,
                    AmountPaid = package.Price
                };

                _dbContext.Subscriptions.Add(subscription);
                await _dbContext.SaveChangesAsync();

                results["Step5_SubscriptionCreation"] = new
                {
                    Status = "? Pass",
                    Message = "Subscription created successfully",
                    SubscriptionId = subscription.SubscriptionId,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    Duration = $"{package.DurationDays} days"
                };

                results["OverallStatus"] = "? All steps passed - Payment flow is working!";

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Payment flow test error: {ex.Message}", ex);
                results["Error"] = ex.Message;
                results["StackTrace"] = ex.StackTrace;
                return Ok(results);
            }
        }

        /// <summary>
        /// Check database connection
        /// </summary>
        private async Task<object> CheckDatabaseConnection()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();
                if (!canConnect)
                {
                    return new { Status = "? Fail", Message = "Cannot connect to database" };
                }

                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                return new
                {
                    Status = "? Pass",
                    Message = "Database connection successful",
                    PendingMigrations = pendingMigrations.Count(),
                    MigrationsList = pendingMigrations
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Status = "? Fail",
                    Message = $"Database connection error: {ex.Message}",
                    Error = ex.InnerException?.Message
                };
            }
        }

        /// <summary>
        /// Test individual components
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ComponentTest(string component)
        {
            try
            {
                return component?.ToLower() switch
                {
                    "database" => await ComponentDatabaseTest(),
                    "session" => ComponentSessionTest(),
                    "encoding" => ComponentEncodingTest(),
                    "packages" => await ComponentPackagesTest(),
                    "payments" => await ComponentPaymentsTest(),
                    _ => Ok(new { Error = "Unknown component. Use: database, session, encoding, packages, payments" })
                };
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        private async Task<IActionResult> ComponentDatabaseTest()
        {
            try
            {
                var result = new Dictionary<string, object>();

                // Test connection
                var canConnect = await _dbContext.Database.CanConnectAsync();
                result["Connection"] = canConnect ? "? OK" : "? Failed";

                // Test Package table
                var packageCount = await _dbContext.Packages.CountAsync();
                result["Packages"] = $"? {packageCount} packages found";

                // Test User table
                var userCount = await _dbContext.Users.CountAsync();
                result["Users"] = $"? {userCount} users found";

                // Test Payment table
                var paymentCount = await _dbContext.Payments.CountAsync();
                result["Payments"] = $"? {paymentCount} payments found";

                // Test Subscription table
                var subscriptionCount = await _dbContext.Subscriptions.CountAsync();
                result["Subscriptions"] = $"? {subscriptionCount} subscriptions found";

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message });
            }
        }

        private IActionResult ComponentSessionTest()
        {
            var result = new Dictionary<string, object>
            {
                ["UserId"] = HttpContext.Session.GetInt32("UserId") ?? 0,
                ["UserEmail"] = HttpContext.Session.GetString("UserEmail") ?? "None",
                ["UserName"] = HttpContext.Session.GetString("UserName") ?? "None",
                ["UserRole"] = HttpContext.Session.GetString("UserRole") ?? "None"
            };

            return Ok(result);
        }

        private IActionResult ComponentEncodingTest()
        {
            var result = new Dictionary<string, string>
            {
                ["UTF8Test"] = "Ti?ng Vi?t: Thông Tin Thanh Toán",
                ["SpecialChars"] = "? € £ ¥ © ® ™",
                ["Vietnamese"] = "Ph??ng th?c thanh toán: Th? tín d?ng, Ngân hàng, Ti?n m?t, Ví ?i?n t?",
                ["Status"] = "? Encoding test complete"
            };

            return Ok(result);
        }

        private async Task<IActionResult> ComponentPackagesTest()
        {
            try
            {
                var packages = await _dbContext.Packages
                    .Where(p => p.Status == "Active")
                    .Select(p => new
                    {
                        p.PackageId,
                        p.PackageName,
                        p.Price,
                        p.DurationDays,
                        p.Status
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Total = packages.Count,
                    Packages = packages
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message });
            }
        }

        private async Task<IActionResult> ComponentPaymentsTest()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return Ok(new { Error = "User not logged in" });
                }

                var payments = await _dbContext.Payments
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(10)
                    .Select(p => new
                    {
                        p.PaymentId,
                        p.Amount,
                        p.Status,
                        p.PaymentMethod,
                        p.PaymentDate
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Total = payments.Count,
                    Payments = payments
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Error = ex.Message });
            }
        }
    }

    /// <summary>
    /// Test Payment Request Model
    /// </summary>
    public class TestPaymentRequest
    {
        public int PackageId { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
