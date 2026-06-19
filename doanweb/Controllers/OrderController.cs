using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;
using doanweb.Services;

namespace doanweb.Controllers
{
    public class OrderController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<OrderController> _logger;
        private readonly IInvoiceService _invoiceService;

        public OrderController(GymDbContext dbContext, ILogger<OrderController> logger, IInvoiceService invoiceService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
        }

        // GET: Trang đặt hàng sản phẩm thành công
        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = $"/Order/Success/{id}" });
                }

                var order = await _dbContext.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId.Value);

                if (order == null)
                {
                    _logger.LogWarning($"Order ID {id} not found for user {userId}");
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng";
                    return RedirectToAction("MyOrders");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order success page for order {OrderId}", id);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải trang đơn hàng";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Download hóa đơn PDF
        [HttpGet]
        public async Task<IActionResult> DownloadInvoice(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return Unauthorized();
                }

                var order = await _dbContext.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId.Value);

                if (order == null)
                {
                    return NotFound();
                }

                var orderItems = order.OrderItems?.ToList() ?? new List<OrderItem>();
                if (orderItems.Count == 0)
                {
                    _logger.LogWarning("Order {OrderId} has no items for invoice", id);
                }

                var pdfBytes = _invoiceService.GenerateInvoicePdf(order, orderItems);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return StatusCode(500, "Không tạo được file PDF hóa đơn");
                }

                return File(pdfBytes, "application/pdf", $"HoaDon_{order.OrderId}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice PDF for order {OrderId}: {Message}", id, ex.Message);
                return StatusCode(500, "Lỗi khi tạo hóa đơn");
            }
        }

        // GET: ??n hàng c?a tôi
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/Order/MyOrders" });
                }

                var orders = await _dbContext.Orders
                    .Where(o => o.UserId == userId.Value)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading orders: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải đơn hàng";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Chi ti?t ??n hàng
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = $"/Order/Details/{id}" });
                }

                var order = await _dbContext.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId.Value);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng";
                    return RedirectToAction("MyOrders");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading order details: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải chi tiết đơn hàng";
                return RedirectToAction("MyOrders");
            }
        }

        // GET: Gói t?p c?a tôi
        [HttpGet]
        public async Task<IActionResult> MyPackages()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/Order/MyPackages" });
                }

                var subscriptions = await _dbContext.Subscriptions
                    .Where(s => s.UserId == userId.Value)
                    .Include(s => s.Package)
                    .OrderByDescending(s => s.CreatedDate)
                    .ToListAsync();

                return View(subscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading packages: {ex.Message}");
                TempData["ErrorMessage"] = "đã xảy ra lỗi khi tải gói t?p";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
