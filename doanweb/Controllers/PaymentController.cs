using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;
using System.Linq;

namespace doanweb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(GymDbContext dbContext, ILogger<PaymentController> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Trang Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout(int packageId)
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
        public async Task<IActionResult> Checkout(int packageId, string packageName, decimal price, int durationDays, string paymentMethod, string transactionId, string notes)
        {
            try
            {
                _logger.LogInformation($"Payment checkout started: packageId={packageId}, paymentMethod={paymentMethod}");

                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("User not logged in");
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục";
                    return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = $"/Payment/Checkout?packageId={packageId}" });
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
                    return RedirectToAction("Checkout", new { packageId = packageId });
                }

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
                    Notes = notes
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
                    TransactionId = transactionId,
                    Status = "Success",
                    Description = $"Thanh toán cho gói {packageName}",
                    Notes = notes
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
                return RedirectToAction("Checkout", new { packageId = packageId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Checkout POST: {ex.Message}\n{ex.StackTrace}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xử lý thanh toán: " + ex.Message;
                return RedirectToAction("Checkout", new { packageId = packageId });
            }
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
