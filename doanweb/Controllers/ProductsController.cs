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
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        [HttpPost]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Return JSON response for JavaScript to handle
            return Ok(new
            {
                success = true,
                productId = product.ProductId,
                productName = product.ProductName,
                price = product.Price,
                imageUrl = product.ImageUrl,
                quantity = quantity
            });
        }
    }
}
