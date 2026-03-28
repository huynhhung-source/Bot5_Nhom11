using Microsoft.AspNetCore.Mvc;
using doanweb.Data;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Controllers
{
    public class OrderController : Controller
    {
        private readonly GymDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(GymDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Order/Success/{id}
        public async Task<IActionResult> Success(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account", new { area = "Customer" });
                }

                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId.Value);

                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Order/Success] Error: {ex.Message}", ex);
                return NotFound();
            }
        }

        // GET: Order/MyOrders - Xem ??n hàng c?a tôi
        public async Task<IActionResult> MyOrders()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account", new { area = "Customer" });
                }

                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Where(o => o.UserId == userId.Value)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Order/MyOrders] Error: {ex.Message}", ex);
                return NotFound();
            }
        }

        // GET: Order/Details/{id} - Chi ti?t ??n hàng
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account", new { area = "Customer" });
                }

                var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId.Value);

                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Order/Details] Error: {ex.Message}", ex);
                return NotFound();
            }
        }

        // POST: Order/CancelOrder/{id}
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return Unauthorized();
                }

                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId.Value);

                if (order == null)
                {
                    return NotFound();
                }

                // Ch? có th? h?y n?u tr?ng thái là Pending
                if (order.Status != "Pending")
                {
                    TempData["ErrorMessage"] = "Ch? có th? h?y ??n hàng ? tr?ng thái ch? x? lý";
                    return RedirectToAction("Details", new { id = order.OrderId });
                }

                order.Status = "Cancelled";
                _context.Update(order);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "??n hàng ?ã b? h?y";
                return RedirectToAction("MyOrders");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Order/CancelOrder] Error: {ex.Message}", ex);
                TempData["ErrorMessage"] = "L?i khi h?y ??n hàng";
                return RedirectToAction("MyOrders");
            }
        }
    }
}
