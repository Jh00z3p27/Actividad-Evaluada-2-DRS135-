using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApp.Data;
using EcommerceApp.Models;
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
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Sizes)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query.ToListAsync();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int size)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                // Le pasamos a CartService los parámetros requeridos para la preorden
                CartService.Instance.AddItem(product.Id, product.Name, (int)product.Price, product.ImageUrl, size);
                
                TempData["Success"] = $"¡{product.Name} (Talla {size}) agregado a la preorden!";
            }
            return RedirectToAction("Index");
        }
    }
}