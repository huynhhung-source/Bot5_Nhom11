using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;

namespace doanweb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly GymDbContext _dbContext;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(GymDbContext dbContext, ILogger<PaymentController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Ki?m tra xem user ?ã ??ng nh?p hay ch?a
        [HttpGet]
        public IActionResult CheckLogin()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var isLoggedIn = userId.HasValue && userId.Value > 0;
                
                return Json(new { isLoggedIn = isLoggedIn });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CheckLogin: {ex.Message}", ex);
                return Json(new { isLoggedIn = false });
            }
        }

        // Trang thanh toán
        [HttpGet]
        public async Task<IActionResult> Checkout(int packageId)
        {
            try
            {
                _logger.LogInformation($"[Checkout GET] packageId={packageId}");

                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    _logger.LogInformation($"[Checkout GET] User not logged in, redirecting to login");
                    return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = $"/payment/checkout?packageId={packageId}" });
                }

                _logger.LogInformation($"[Checkout GET] userId={userId.Value}");

                var package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.PackageId == packageId && p.Status == "Active");
                if (package == null)
                {
                    _logger.LogWarning($"[Checkout GET] Package not found or inactive: packageId={packageId}");
                    TempData["ErrorMessage"] = "? Gói t?p không t?n t?i ho?c ?ã b? vô hi?u hóa";
                    return RedirectToAction("Online", "Packages", new { area = "" });
                }

                _logger.LogInformation($"[Checkout GET] Package found: {package.PackageName}, Price={package.Price}");

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
                _logger.LogError($"[Checkout GET] Error: {ex.Message}\n{ex.StackTrace}", ex);
                TempData["ErrorMessage"] = "? ?ã x?y ra l?i khi t?i trang thanh toán";
                return RedirectToAction("Online", "Packages", new { area = "" });
            }
        }

        // X? lý thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(PaymentViewModel model)
        {
            try
            {
                _logger.LogInformation($"[Checkout POST] Starting payment process");

                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    _logger.LogWarning($"[Checkout POST] User not logged in");
                    return RedirectToAction("Login", "Account", new { area = "Customer" });
                }

                _logger.LogInformation($"[Checkout POST] userId={userId.Value}");

                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    _logger.LogWarning($"[Checkout POST] Invalid model state: {errors}");
                    return View(model);
                }

                // Validate model data
                if (model == null || model.PackageId <= 0 || model.Price <= 0 || model.DurationDays <= 0)
                {
                    _logger.LogWarning($"[Checkout POST] Invalid payment model data: PackageId={model?.PackageId}, Price={model?.Price}, DurationDays={model?.DurationDays}");
                    ModelState.AddModelError("", "Thông tin thanh toán không h?p l?");
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.PaymentMethod))
                {
                    _logger.LogWarning($"[Checkout POST] Payment method not selected");
                    ModelState.AddModelError("PaymentMethod", "Vui lòng ch?n ph??ng th?c thanh toán");
                    return View(model);
                }

                var package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.PackageId == model.PackageId && p.Status == "Active");
                if (package == null)
                {
                    _logger.LogWarning($"[Checkout POST] Package not found: {model.PackageId}");
                    ModelState.AddModelError("", "Gói t?p không t?n t?i ho?c ?ã b? vô hi?u hóa");
                    return View(model);
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value && u.Status == "Active");
                if (user == null)
                {
                    _logger.LogWarning($"[Checkout POST] User not found or inactive: {userId.Value}");
                    ModelState.AddModelError("", "Ng??i dùng không t?n t?i ho?c ?ã b? vô hi?u hóa");
                    return View(model);
                }

                _logger.LogInformation($"[Checkout POST] Creating payment for user {user.Email}, package {package.PackageName}");

                // T?o thanh toán
                var payment = new Payment
                {
                    UserId = userId.Value,
                    Amount = model.Price,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = model.PaymentMethod,
                    TransactionId = string.IsNullOrEmpty(model.TransactionId) ? GenerateTransactionId() : model.TransactionId,
                    Status = "Success",
                    Description = $"Thanh toán gói t?p: {package.PackageName}",
                    Notes = model.Notes
                };

                _dbContext.Payments.Add(payment);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"[Checkout POST] Payment created: PaymentId={payment.PaymentId}");

                // T?o subscription (??ng ký gói)
                var subscription = new Subscription
                {
                    UserId = userId.Value,
                    PackageId = model.PackageId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(package.DurationDays),
                    ActivationDate = DateTime.Now,
                    Status = "Active",
                    RemainingDays = package.DurationDays,
                    SessionsUsed = 0,
                    AmountPaid = model.Price,
                    Notes = $"Thanh toán qua {model.PaymentMethod}"
                };

                _dbContext.Subscriptions.Add(subscription);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"[Checkout POST] Subscription created: SubscriptionId={subscription.SubscriptionId}");
                _logger.LogInformation($"[Checkout POST] Payment successful: User {user.Email}, Amount {model.Price}, Package {package.PackageName}");

                TempData["SuccessMessage"] = "? Thanh toán thành công! Gói t?p c?a b?n ?ã ???c kích ho?t.";
                return RedirectToAction("PaymentSuccess", "Payment", new { area = "", paymentId = payment.PaymentId, subscriptionId = subscription.SubscriptionId });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError($"[Checkout POST] Database error: {dbEx.InnerException?.Message ?? dbEx.Message}\n{dbEx.StackTrace}", dbEx);
                ModelState.AddModelError("", "L?i c? s? d? li?u. Vui lòng th? l?i sau ho?c liên h? h? tr?.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Checkout POST] Unexpected error: {ex.Message}\n{ex.StackTrace}", ex);
                ModelState.AddModelError("", "?ã x?y ra l?i khi x? lý thanh toán. Vui lòng th? l?i ho?c liên h? h? tr?.");
                return View(model);
            }
        }

        // Trang thành công
        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(int paymentId, int subscriptionId)
        {
            try
            {
                _logger.LogInformation($"[PaymentSuccess] paymentId={paymentId}, subscriptionId={subscriptionId}");

                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    _logger.LogWarning($"[PaymentSuccess] User not logged in");
                    return RedirectToAction("Login", "Account", new { area = "Customer" });
                }

                var payment = await _dbContext.Payments
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId && p.UserId == userId.Value);

                if (payment == null)
                {
                    _logger.LogWarning($"[PaymentSuccess] Payment not found: paymentId={paymentId}, userId={userId.Value}");
                    return NotFound();
                }

                var subscription = await _dbContext.Subscriptions
                    .Include(s => s.Package)
                    .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId && s.UserId == userId.Value);

                if (subscription == null)
                {
                    _logger.LogWarning($"[PaymentSuccess] Subscription not found: subscriptionId={subscriptionId}, userId={userId.Value}");
                }

                _logger.LogInformation($"[PaymentSuccess] Success page loaded for user {payment.User?.Email}");

                ViewBag.Payment = payment;
                ViewBag.Subscription = subscription;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PaymentSuccess] Error: {ex.Message}\n{ex.StackTrace}", ex);
                return NotFound();
            }
        }

        // Thanh to?n s?n ph?m (t? gi? h?ng)
        [HttpPost]
        public async Task<IActionResult> CheckoutProducts([FromBody] CheckoutRequest request)
        {
            try
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return Unauthorized(new { message = "Vui lòng ??ng nh?p" });
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);
        if (user == null)
        {
            return NotFound(new { message = "Ng??i dùng không tìm th?y" });
        }

        if (request.CartItems == null || !request.CartItems.Any())
        {
            return BadRequest(new { message = "Gi? hàng tr?ng" });
        }

        // Tính t?ng ti?n
        decimal totalAmount = request.CartItems.Sum(item => item.price * item.quantity);

        // T?o ??n hàng
        var order = new Order
        {
            UserId = userId.Value,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount,
            DeliveryAddress = request.deliveryAddress,
            Status = "Pending", // Ch? x? lý
            Notes = request.notes
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // T?o chi ti?t ??n hàng
        foreach (var item in request.CartItems)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == item.productId);
            if (product != null)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.productId,
                    Quantity = item.quantity,
                    UnitPrice = item.price,
                    TotalPrice = item.price * item.quantity
                };

                _dbContext.OrderItems.Add(orderItem);

                // C?p nh?t t?n kho
                product.StockQuantity -= item.quantity;
            }
        }

        await _dbContext.SaveChangesAsync();

        // T?o thanh toán
        var payment = new Payment
        {
            UserId = userId.Value,
            Amount = totalAmount,
            PaymentDate = DateTime.Now,
            PaymentMethod = request.paymentMethod,
            TransactionId = GenerateTransactionId(),
            Status = "Success",
            Description = $"Thanh toán mua hàng {request.CartItems.Count} s?n ph?m",
            Notes = request.notes
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"[CheckoutProducts] Order created: OrderId={order.OrderId}, UserId={userId.Value}, Amount={totalAmount}");

        return Ok(new 
        { 
            success = true, 
            orderId = order.OrderId,
            message = "??n hàng ???c t?o thành công"
        });
    }
    catch (Exception ex)
    {
        _logger.LogError($"[CheckoutProducts] Error: {ex.Message}", ex);
        return StatusCode(500, new { message = "L?i khi x? lý thanh toán" });
    }
}

        // Hàm t?o mã giao d?ch
        private string GenerateTransactionId()
        {
            return $"TXN{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }
    }
}
