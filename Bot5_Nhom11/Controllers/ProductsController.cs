using Microsoft.AspNetCore.Mvc;
using doanweb.Data;
using doanweb.Models;
using Microsoft.EntityFrameworkCore;

namespace doanweb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly GymDbContext _context;

        public ProductsController(GymDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => p.Status == "Active")
                .ToListAsync();
            
            return View(products);
        }

        // GET: Products/Category
        public async Task<IActionResult> Category(string categoryName)
        {
            var products = await _context.Products
                .Where(p => p.Status == "Active" && p.Category == categoryName)
                .ToListAsync();

            ViewBag.CategoryName = categoryName;
            return View("Index", products);
        }

        // GET: Products/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy các sản phẩm tương tự trong cùng category
            var relatedProducts = await _context.Products
                .Where(p => p.Category == product.Category && p.ProductId != product.ProductId && p.Status == "Active")
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;
            return View(product);
        }

        // GET: Products/Search
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction("Index");
            }

            var products = await _context.Products
                .Where(p => p.Status == "Active" && 
                       (p.ProductName.Contains(searchTerm) || 
                        p.Description.Contains(searchTerm) || 
                        p.Category.Contains(searchTerm)))
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            return View("Index", products);
        }

        /// <summary>
        /// Lấy tồn kho một sản phẩm
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Stock(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.Status != "Active")
            {
                return Json(new { productId = id, stockQuantity = 0, inStock = false });
            }

            return Json(new
            {
                productId = product.ProductId,
                stockQuantity = product.StockQuantity,
                inStock = product.StockQuantity > 0
            });
        }

        /// <summary>
        /// Lấy tồn kho nhiều sản phẩm (cho giỏ hàng)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> StockBatch(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                return Json(Array.Empty<object>());
            }

            var productIds = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .Select(p => new { p.ProductId, p.StockQuantity, p.Status })
                .ToListAsync();

            var result = productIds.Select(id =>
            {
                var product = products.FirstOrDefault(p => p.ProductId == id);
                return new
                {
                    productId = id,
                    stockQuantity = product?.StockQuantity ?? 0,
                    status = product?.Status ?? "Inactive"
                };
            }).ToList();

            return Json(result);
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng (kiểm tra tồn kho)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null || product.Status != "Active")
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc ngừng bán" });
            }

            if (product.StockQuantity <= 0)
            {
                return Json(new { success = false, message = "Sản phẩm đã hết hàng" });
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            if (quantity > product.StockQuantity)
            {
                return Json(new
                {
                    success = false,
                    message = $"{product.ProductName} chỉ còn {product.StockQuantity} sản phẩm trong kho",
                    stockQuantity = product.StockQuantity
                });
            }

            return Json(new
            {
                success = true,
                productId = product.ProductId,
                productName = product.ProductName,
                price = product.Price,
                imageUrl = product.ImageUrl,
                stockQuantity = product.StockQuantity,
                quantity = quantity
            });
        }
    }
}
