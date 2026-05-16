using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;

namespace doanweb.Controllers
{
    public class OrderController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<OrderController> _logger;

        public OrderController(GymDbContext dbContext, ILogger<OrderController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Ki?m tra xem user ?ã ??ng nh?p ch?a
        private bool IsUserLoggedIn(out int userId)
        {
            userId = 0;
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return false;
            }
            userId = userIdSession.Value;
            return true;
        }

        /// <summary>
        /// Hi?n th? t?t c? các gói t?p mà khách hàng ?ã mua
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyPackages()
        {
            if (!IsUserLoggedIn(out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/Order/MyPackages" });
            }

            try
            {
                // L?y t?t c? subscription c?a user
                var subscriptions = await _dbContext.Subscriptions
                    .Where(s => s.UserId == userId)
                    .Include(s => s.Package)
                    .Include(s => s.User)
                    .OrderByDescending(s => s.StartDate)
                    .ToListAsync();

                _logger.LogInformation($"User {userId} accessed MyPackages. Found {subscriptions.Count} packages");

                return View(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MyPackages: {ex.Message}");
                TempData["ErrorMessage"] = "?ã x?y ra l?i khi t?i danh sách gói t?p";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Hi?n th? chi ti?t m?t gói t?p c? th?
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> PackageDetail(int subscriptionId)
        {
            if (!IsUserLoggedIn(out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = $"/Order/PackageDetail/{subscriptionId}" });
            }

            try
            {
                var subscription = await _dbContext.Subscriptions
                    .Where(s => s.SubscriptionId == subscriptionId && s.UserId == userId)
                    .Include(s => s.Package)
                    .Include(s => s.User)
                    .Include(s => s.Attendances)
                    .FirstOrDefaultAsync();

                if (subscription == null)
                {
                    TempData["ErrorMessage"] = "Gói t?p không t?n t?i ho?c b?n không có quy?n truy c?p";
                    return RedirectToAction("MyPackages");
                }

                return View(subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PackageDetail: {ex.Message}");
                TempData["ErrorMessage"] = "?ã x?y ra l?i khi t?i chi ti?t gói t?p";
                return RedirectToAction("MyPackages");
            }
        }

        /// <summary>
        /// L?y danh sách các gói t?p theo tr?ng thái (API endpoint)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPackagesByStatus(string status = "Active")
        {
            if (!IsUserLoggedIn(out int userId))
            {
                return Json(new { success = false, message = "B?n ch?a ??ng nh?p" });
            }

            try
            {
                var query = _dbContext.Subscriptions
                    .Where(s => s.UserId == userId)
                    .Include(s => s.Package)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(s => s.Status == status);
                }

                var packages = await query
                    .OrderByDescending(s => s.StartDate)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    count = packages.Count,
                    data = packages.Select(s => new
                    {
                        subscriptionId = s.SubscriptionId,
                        packageName = s.Package?.PackageName,
                        status = s.Status,
                        startDate = s.StartDate.ToString("dd/MM/yyyy"),
                        endDate = s.EndDate.ToString("dd/MM/yyyy"),
                        remainingDays = (int)(s.EndDate - DateTime.Now).TotalDays,
                        amountPaid = s.AmountPaid,
                        sessionsUsed = s.SessionsUsed
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetPackagesByStatus: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Hi?n th? danh sách ??n hàng s?n ph?m c?a khách hàng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            if (!IsUserLoggedIn(out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/Order/MyOrders" });
            }

            try
            {
                var orders = await _dbContext.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                _logger.LogInformation($"User {userId} accessed MyOrders. Found {orders.Count} orders");

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MyOrders: {ex.Message}");
                TempData["ErrorMessage"] = "?ã x?y ra l?i khi t?i danh sách ??n hàng";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Hi?n th? chi ti?t m?t ??n hàng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsUserLoggedIn(out int userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = $"/Order/Details/{id}" });
            }

            try
            {
                var order = await _dbContext.Orders
                    .Where(o => o.OrderId == id && o.UserId == userId)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    TempData["ErrorMessage"] = "??n hàng không t?n t?i ho?c b?n không có quy?n truy c?p";
                    return RedirectToAction("MyOrders");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Details: {ex.Message}");
                TempData["ErrorMessage"] = "?ã x?y ra l?i khi t?i chi ti?t ??n hàng";
                return RedirectToAction("MyOrders");
            }
        }

        /// <summary>
        /// L?y th?ng kê ??n hàng c?a khách hàng (API endpoint)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOrderStats()
        {
            if (!IsUserLoggedIn(out int userId))
            {
                return Json(new { success = false, message = "B?n ch?a ??ng nh?p" });
            }

            try
            {
                var stats = await _dbContext.Orders
                    .Where(o => o.UserId == userId)
                    .GroupBy(o => o.Status)
                    .Select(g => new
                    {
                        status = g.Key,
                        count = g.Count(),
                        totalAmount = g.Sum(o => o.TotalAmount)
                    })
                    .ToListAsync();

                var totalOrders = await _dbContext.Orders
                    .Where(o => o.UserId == userId)
                    .CountAsync();

                var totalSpent = await _dbContext.Orders
                    .Where(o => o.UserId == userId)
                    .SumAsync(o => o.TotalAmount);

                return Json(new
                {
                    success = true,
                    totalOrders = totalOrders,
                    totalSpent = totalSpent,
                    byStatus = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetOrderStats: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// H?y ??n hàng (ch? khi ??n hàng ? tr?ng thái Pending)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            if (!IsUserLoggedIn(out int userId))
            {
                return Json(new { success = false, message = "B?n ch?a ??ng nh?p" });
            }

            try
            {
                var order = await _dbContext.Orders
                    .Where(o => o.OrderId == orderId && o.UserId == userId)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return Json(new { success = false, message = "??n hàng không t?n t?i" });
                }

                if (order.Status != "Pending")
                {
                    return Json(new { success = false, message = "Ch? có th? h?y ??n hàng ? tr?ng thái Ch? Xác Nh?n" });
                }

                order.Status = "Cancelled";
                _dbContext.Orders.Update(order);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Order {orderId} cancelled by user {userId}");

                return Json(new { success = true, message = "??n hàng ?ã ???c h?y thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CancelOrder: {ex.Message}");
                return Json(new { success = false, message = "?ã x?y ra l?i khi h?y ??n hàng" });
            }
        }
    }
}
