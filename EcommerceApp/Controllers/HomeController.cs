using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApp.Data;
using EcommerceApp.Services;

namespace EcommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).Include(p => p.Sizes).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategory = categoryId;

            var products = await query.ToListAsync();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int size)
        {
            var product = await _context.Products.Include(p => p.Sizes).FirstOrDefaultAsync(p => p.Id == productId);
            if (product != null)
            {
                var sizeInfo = product.Sizes.FirstOrDefault(s => s.SizeNumber == size);
                if (sizeInfo != null && sizeInfo.Stock > 0)
                {
                    CartService.Instance.AddItem(product.Id, product.Name, size, product.ImageUrl);
                    TempData["Success"] = $"¡Anillo {product.Name} (Talla {size}) agregado al carrito!";
                }
                else
                {
                    TempData["Error"] = "La talla seleccionada no tiene stock disponible.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}