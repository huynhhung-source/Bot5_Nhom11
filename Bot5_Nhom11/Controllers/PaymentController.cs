using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;
using doanweb.Services;
using System.Linq;

namespace doanweb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<PaymentController> _logger;
        private readonly IGymService _gymService;

        public PaymentController(GymDbContext dbContext, ILogger<PaymentController> logger, IGymService gymService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gymService = gymService ?? throw new ArgumentNullException(nameof(gymService));
        }

        // GET: Trang Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout(int packageId, int? gymId = null)
        {
            try
            {
                var package = await _dbContext.Packages.FindAsync(packageId);
                if (package == null || package.Status != "Active")
                {
                    TempData["ErrorMessage"] = "Gói tập không tồn tại hoặc không hợp lệ";
                    return RedirectToAction("Detail", "Packages", new { id = packageId });
                }

                var model = new PaymentViewModel
                {
                    PackageId = package.PackageId,
                    PackageName = package.PackageName,
                    Price = package.Price,
                    DurationDays = package.DurationDays
                };

                AddGymContext(model, gymId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Checkout GET: {ex.Message}\n{ex.StackTrace}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải trang thanh toán";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Xử lý thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(
            int packageId,
            string packageName,
            decimal price,
            int durationDays,
            string paymentMethod,
            string? transactionId,
            string? notes,
            int? gymId,
            string? gymName,
            string? className,
            string? instructorName,
            string? gymAddress,
            string? gymHours)
        {
            try
            {
                _logger.LogInformation($"Payment checkout started: packageId={packageId}, paymentMethod={paymentMethod}");

                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("User not logged in");
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục";
                    var returnUrl = Url.Action("Checkout", "Payment", new { area = "", packageId, gymId });
                    return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl });
                }

                // Kiểm tra gói tập
                var package = await _dbContext.Packages.FindAsync(packageId);
                if (package == null || package.Status != "Active")
                {
                    _logger.LogWarning($"Package {packageId} not found or inactive");
                    TempData["ErrorMessage"] = "Gói tập không tồn tại hoặc không khả dụng";
                    return RedirectToAction("Detail", "Packages", new { id = packageId });
                }

                if (string.IsNullOrEmpty(paymentMethod))
                {
                    _logger.LogWarning("No payment method selected");
                    TempData["ErrorMessage"] = "Vui lòng chọn phương thức thanh toán";
                    return RedirectToAction("Checkout", new { packageId, gymId });
                }

                var gymContext = ResolveGymContext(gymId, gymName, className, instructorName, gymAddress, gymHours);
                var trimmedNotes = TrimTo(notes, 255);
                var subscriptionNotes = TrimTo(BuildSubscriptionNotes(gymContext, notes), 255);
                var description = TrimTo(BuildPaymentDescription(package.PackageName, gymContext), 500);

                // Tạo Subscription
                var subscription = new Subscription
                {
                    UserId = userId.Value,
                    PackageId = packageId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(durationDays),
                    ActivationDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    Status = "Active",
                    RemainingDays = durationDays,
                    SessionsUsed = 0,
                    AmountPaid = price,
                    Notes = subscriptionNotes
                };

                _dbContext.Subscriptions.Add(subscription);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Subscription created: ID={subscription.SubscriptionId}");

                // Tạo Payment
                var payment = new Payment
                {
                    UserId = userId.Value,
                    SubscriptionId = subscription.SubscriptionId,
                    Amount = price,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = paymentMethod,
                    TransactionId = TrimTo(transactionId, 100),
                    Status = "Success",
                    Description = description,
                    Notes = trimmedNotes
                };

                _dbContext.Payments.Add(payment);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Payment created: ID={payment.PaymentId}, User={userId}");

                // Redirect về controller Payment gốc (không phải Admin area)
                return RedirectToAction("Success", "Payment", new { area = "", paymentId = payment.PaymentId });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError($"Database error in Checkout POST: {dbEx.Message}\n{dbEx.InnerException?.Message}");
                TempData["ErrorMessage"] = "Lỗi cơ sở dữ liệu khi xử lý thanh toán";
                return RedirectToAction("Checkout", new { packageId, gymId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Checkout POST: {ex.Message}\n{ex.StackTrace}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xử lý thanh toán: " + ex.Message;
                return RedirectToAction("Checkout", new { packageId, gymId });
            }
        }

        private void AddGymContext(PaymentViewModel model, int? gymId)
        {
            if (!gymId.HasValue)
            {
                return;
            }

            var gym = _gymService.GetById(gymId.Value);
            if (gym == null)
            {
                return;
            }

            model.GymId = gym.Id;
            model.GymName = gym.Name;
            model.ClassName = gym.ClassName;
            model.InstructorName = gym.InstructorName;
            model.GymAddress = gym.Address;
            model.GymHours = gym.Hours;
        }

        private PaymentViewModel ResolveGymContext(
            int? gymId,
            string? gymName,
            string? className,
            string? instructorName,
            string? gymAddress,
            string? gymHours)
        {
            var model = new PaymentViewModel
            {
                GymId = gymId,
                GymName = gymName,
                ClassName = className,
                InstructorName = instructorName,
                GymAddress = gymAddress,
                GymHours = gymHours
            };

            AddGymContext(model, gymId);
            return model;
        }

        private static string BuildPaymentDescription(string packageName, PaymentViewModel gymContext)
        {
            if (!gymContext.IsGymCheckout)
            {
                return $"Thanh toán cho gói {packageName}";
            }

            var parts = new List<string> { $"Thanh toán phòng tập - gói {packageName}" };

            if (!string.IsNullOrWhiteSpace(gymContext.GymName))
            {
                parts.Add(gymContext.GymName);
            }

            if (!string.IsNullOrWhiteSpace(gymContext.ClassName))
            {
                parts.Add($"Lớp {gymContext.ClassName}");
            }

            return string.Join(" | ", parts);
        }

        private static string? BuildSubscriptionNotes(PaymentViewModel gymContext, string? notes)
        {
            var parts = new List<string>();

            if (gymContext.IsGymCheckout)
            {
                if (!string.IsNullOrWhiteSpace(gymContext.GymName))
                {
                    parts.Add($"Phòng tập: {gymContext.GymName}");
                }

                if (!string.IsNullOrWhiteSpace(gymContext.ClassName))
                {
                    parts.Add($"Lớp: {gymContext.ClassName}");
                }

                if (!string.IsNullOrWhiteSpace(gymContext.InstructorName))
                {
                    parts.Add($"HLV: {gymContext.InstructorName}");
                }
            }

            if (!string.IsNullOrWhiteSpace(notes))
            {
                parts.Add(notes.Trim());
            }

            return parts.Count == 0 ? null : string.Join("; ", parts);
        }

        private static string? TrimTo(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
        }

        // GET: Payment Success page
        [HttpGet]
        public async Task<IActionResult> Success(int paymentId)
        {
            try
            {
                var payment = await _dbContext.Payments
                    .Include(p => p.Subscription)
                    .ThenInclude(s => s.Package)
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                if (payment == null)
                {
                    _logger.LogWarning($"Payment {paymentId} not found");
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin thanh toán";
                    return RedirectToAction("Index", "Home");
                }

                // Verify that the payment belongs to the current user
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue || payment.UserId != userId.Value)
                {
                    _logger.LogWarning($"Unauthorized access to payment {paymentId}");
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập thanh toán này";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Payment = payment;
                ViewBag.Subscription = payment.Subscription;

                return View("PaymentSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Payment Success: {ex.Message}\n{ex.StackTrace}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải trang thanh toán";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Thanh toán sản phẩm từ giỏ hàng (JSON API)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CheckoutProducts([FromBody] CheckoutRequest request)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thanh toán" });
                }

                if (request?.CartItems == null || request.CartItems.Count == 0)
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                if (string.IsNullOrWhiteSpace(request.deliveryAddress))
                {
                    return Json(new { success = false, message = "Vui lòng nhập địa chỉ giao hàng" });
                }

                if (string.IsNullOrWhiteSpace(request.paymentMethod))
                {
                    return Json(new { success = false, message = "Vui lòng chọn phương thức thanh toán" });
                }

                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in request.CartItems)
                {
                    if (item.quantity <= 0)
                    {
                        return Json(new { success = false, message = "Số lượng sản phẩm không hợp lệ" });
                    }

                    var product = await _dbContext.Products.FindAsync(item.productId);
                    if (product == null || product.Status != "Active")
                    {
                        return Json(new { success = false, message = $"Sản phẩm '{item.productName}' không tồn tại hoặc ngừng bán" });
                    }

                    if (product.StockQuantity < item.quantity)
                    {
                        return Json(new { success = false, message = $"{product.ProductName} chỉ còn {product.StockQuantity} sản phẩm trong kho" });
                    }

                    var unitPrice = product.Price;
                    var lineTotal = unitPrice * item.quantity;
                    totalAmount += lineTotal;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = item.quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = lineTotal
                    });

                    product.StockQuantity -= item.quantity;
                    product.UpdatedDate = DateTime.Now;
                }

                var isCash = string.Equals(request.paymentMethod, "Cash", StringComparison.OrdinalIgnoreCase);
                var order = new Order
                {
                    UserId = userId.Value,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    Status = isCash ? "Confirmed" : "Pending",
                    DeliveryAddress = request.deliveryAddress.Trim(),
                    Notes = request.notes
                };

                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();

                foreach (var orderItem in orderItems)
                {
                    orderItem.OrderId = order.OrderId;
                    _dbContext.OrderItems.Add(orderItem);
                }

                var transactionId = string.IsNullOrWhiteSpace(request.transactionId)
                    ? null
                    : request.transactionId.Trim();

                var payment = new Payment
                {
                    UserId = userId.Value,
                    Amount = totalAmount,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = request.paymentMethod,
                    TransactionId = transactionId,
                    Status = isCash ? "Success" : "Pending",
                    Description = $"Thanh toán đơn hàng #{order.OrderId}",
                    Notes = request.notes
                };

                _dbContext.Payments.Add(payment);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Product checkout success: OrderId={OrderId}, UserId={UserId}, Amount={Amount}",
                    order.OrderId, userId.Value, totalAmount);

                return Json(new { success = true, orderId = order.OrderId });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error in CheckoutProducts");
                return Json(new { success = false, message = "Lỗi cơ sở dữ liệu khi xử lý đơn hàng" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckoutProducts");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý đơn hàng" });
            }
        }
    }
}
