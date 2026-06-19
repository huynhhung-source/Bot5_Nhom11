using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;

namespace doanweb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(GymDbContext dbContext, ILogger<PaymentController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Ki?m tra quy?n Admin
        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return !string.IsNullOrEmpty(userRole) && userRole == "Admin";
        }

        // Danh sách thanh toán
        [HttpGet]
        public async Task<IActionResult> Index(string status = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/admin/payment/index" });
            }

            var query = _dbContext.Payments
                .Include(p => p.User)
                .AsQueryable();

            // L?c theo tr?ng thái
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Status == status);
            }

            // L?c theo ngày
            if (startDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate <= endDate.Value.AddDays(1));
            }

            var payments = await query.OrderByDescending(p => p.PaymentDate).ToListAsync();

            // Tính t?ng doanh thu
            var totalRevenue = payments.Sum(p => p.Amount);
            var successPayments = payments.Count(p => p.Status == "Success");
            var failedPayments = payments.Count(p => p.Status == "Failed");

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.SuccessPayments = successPayments;
            ViewBag.FailedPayments = failedPayments;
            ViewBag.TotalPayments = payments.Count;
            ViewBag.Status = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(payments);
        }

        // Chi ti?t thanh toán
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var payment = await _dbContext.Payments
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            // L?y thông tin subscription liên quan
            var subscription = await _dbContext.Subscriptions
                .Include(s => s.Package)
                .FirstOrDefaultAsync(s => s.UserId == payment.UserId && s.AmountPaid == payment.Amount);

            ViewBag.Subscription = subscription;

            return View(payment);
        }

        // C?p nh?t tr?ng thái thanh toán
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (!IsAdmin())
            {
                return Json(new { success = false, message = "Không có quyền" });
            }

            try
            {
                var payment = await _dbContext.Payments.FindAsync(id);
                if (payment == null)
                {
                    return Json(new { success = false, message = "Thanh toán không tồn tại" });
                }

                payment.Status = status;
                _dbContext.Payments.Update(payment);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Payment status updated: ID {id}, Status {status}");

                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating payment status: {ex.Message}");
                return Json(new { success = false, message = $"L?i: {ex.Message}" });
            }
        }

        // Xu?t báo cáo
        [HttpGet]
        public async Task<IActionResult> Report(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }

            var query = _dbContext.Payments.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate <= endDate.Value.AddDays(1));
            }

            var payments = await query
                .Include(p => p.User)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(payments);
        }
    }
}
