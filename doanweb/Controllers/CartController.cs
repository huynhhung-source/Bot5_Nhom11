using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using doanweb.Data;
using doanweb.Models;
using System.Text.Json;

namespace doanweb.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "GymCart";
        private readonly GymDbContext _context;

        public CartController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account", new { area = "Customer", returnUrl = "/Cart" });
            }

            if (TempData["ClearLocalCart"] != null)
            {
                ViewBag.ClearLocalCart = true;
            }

            var cart = GetSessionCart();
            var lines = await BuildCartLinesAsync(cart);
            ViewBag.CartJson = JsonSerializer.Serialize(cart);
            return View(lines);
        }

        // POST: Thêm sản phẩm vào giỏ (session)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Add([FromBody] CartAddRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm vào giỏ hàng" });
            }

            if (request == null || request.ProductId <= 0 || request.Quantity <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu sản phẩm không hợp lệ" });
            }

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null || product.Status != "Active")
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc ngừng bán" });
            }

            if (product.StockQuantity <= 0)
            {
                return Json(new { success = false, message = "Sản phẩm đã hết hàng" });
            }

            var cart = GetSessionCart();
            var existing = cart.FirstOrDefault(c => c.productId == product.ProductId);

            var newQty = request.Quantity;
            if (existing != null)
            {
                newQty = existing.quantity + request.Quantity;
            }

            if (newQty > product.StockQuantity)
            {
                return Json(new
                {
                    success = false,
                    message = $"{product.ProductName} chỉ còn {product.StockQuantity} sản phẩm trong kho",
                    stockQuantity = product.StockQuantity
                });



            }

            if (existing != null)
            {
                existing.quantity = newQty;
                existing.price = product.Price;
                existing.productName = product.ProductName;
                existing.imageUrl = product.ImageUrl;
                existing.stockQuantity = product.StockQuantity;
                existing.outOfStock = false;
                existing.unavailable = false
                    
                    ;
            }
            else
            {
                cart.Add(new CartItemRequest
                {
                    productId = product.ProductId,
                    productName = product.ProductName,
                    price = product.Price,
                    imageUrl = product.ImageUrl,
                    quantity = newQty,
                    stockQuantity = product.StockQuantity,
                    outOfStock = false,
                    unavailable = false
                });
            }

            SaveSessionCart(cart);

            return Json(new
            {
                success = true,
                message = "Đã thêm vào giỏ hàng",
                cart = cart,
                totalCount = cart.Sum(c => c.quantity)
            });
        }

        // GET: Lấy giỏ hàng JSON
        [HttpGet]
        public IActionResult Items()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return Json(Array.Empty<CartItemRequest>());
            }

            return Json(GetSessionCart());
        }

        // POST: Đồng bộ giỏ từ client (localStorage cũ)
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Sync([FromBody] List<CartItemRequest>? items)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            if (items == null || items.Count == 0)
            {
                SaveSessionCart(new List<CartItemRequest>());
                return Json(new { success = true, cart = Array.Empty<CartItemRequest>() });
            }

            var merged = new List<CartItemRequest>();

            foreach (var item in items.Where(i => i.productId > 0))
            {
                var product = await _context.Products.FindAsync(item.productId);
                if (product == null || product.Status != "Active")
                {
                    continue;
                }

                var qty = Math.Min(Math.Max(1, item.quantity), product.StockQuantity);
                if (product.StockQuantity <= 0)
                {
                    continue;
                }

                var existing = merged.FirstOrDefault(m => m.productId == product.ProductId);
                if (existing != null)
                {
                    existing.quantity = Math.Min(existing.quantity + qty, product.StockQuantity);
                }
                else
                {
                    merged.Add(new CartItemRequest
                    {
                        productId = product.ProductId,
                        productName = product.ProductName,
                        price = product.Price,
                        imageUrl = product.ImageUrl ?? item.imageUrl,
                        quantity = qty,
                        stockQuantity = product.StockQuantity,
                        outOfStock = false,
                        unavailable = false
                    });
                }
            }

            SaveSessionCart(merged);
            return Json(new { success = true, cart = merged, totalCount = merged.Sum(c => c.quantity) });
        }

        // POST: Cập nhật số lượng
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateQuantity([FromBody] CartAddRequest request)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var cart = GetSessionCart();
            var item = cart.FirstOrDefault(c => c.productId == request.ProductId);
            if (item == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ" });
            }

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            var qty = Math.Min(Math.Max(1, request.Quantity), product.StockQuantity);
            if (product.StockQuantity <= 0)
            {
                return Json(new { success = false, message = "Sản phẩm đã hết hàng" });
            }

            item.quantity = qty;
            item.stockQuantity = product.StockQuantity;
            item.price = product.Price;
            SaveSessionCart(cart);

            return Json(new { success = true, cart = cart });
        }

        // POST: Xóa sản phẩm
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Remove([FromBody] CartAddRequest request)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var cart = GetSessionCart();
            cart.RemoveAll(c => c.productId == request.ProductId);
            SaveSessionCart(cart);

            return Json(new { success = true, cart = cart });
        }

        // GET: Cart/Clear
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartSessionKey);
            TempData["ClearLocalCart"] = true;
            return RedirectToAction(nameof(Index));
        }

        private List<CartItemRequest> GetSessionCart()
        {
            var json = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<CartItemRequest>();
            }

            return JsonSerializer.Deserialize<List<CartItemRequest>>(json) ?? new List<CartItemRequest>();
        }

        private void SaveSessionCart(List<CartItemRequest> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        private async Task<List<CartLineViewModel>> BuildCartLinesAsync(List<CartItemRequest> cart)
        {
            var lines = new List<CartLineViewModel>();

            foreach (var item in cart.Where(c => c.productId > 0))
            {
                var product = await _context.Products.FindAsync(item.productId);
                if (product == null)
                {
                    continue;
                }

                var isUnavailable = product.Status != "Active";
                var isOutOfStock = isUnavailable || product.StockQuantity <= 0;
                var qty = item.quantity;
                if (!isOutOfStock && qty > product.StockQuantity)
                {
                    qty = product.StockQuantity;
                }

                lines.Add(new CartLineViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl ?? item.imageUrl,
                    Quantity = qty,
                    StockQuantity = product.StockQuantity,
                    IsOutOfStock = isOutOfStock,
                    IsUnavailable = isUnavailable
                });
            }

            // Cập nhật lại session với tồn kho mới nhất
            if (lines.Count > 0)
            {
                var refreshed = lines.Select(l => new CartItemRequest
                {
                    productId = l.ProductId,
                    productName = l.ProductName,
                    price = l.Price,
                    imageUrl = l.ImageUrl,
                    quantity = l.Quantity,
                    stockQuantity = l.StockQuantity,
                    outOfStock = l.IsOutOfStock,
                    unavailable = l.IsUnavailable
                }).ToList();
                SaveSessionCart(refreshed);
            }

            return lines;
        }
    }
}
