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
        public async Task<IActionResult> AddToCart(int productId, int size, int quantity = 1)
        {
            if (quantity < 1) quantity = 1;

            var product = await _context.Products
                .Include(p => p.Sizes)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product != null)
            {
                var productSize = product.Sizes.FirstOrDefault(s => s.SizeNumber == size);
                if (productSize != null)
                {
                    // 1. Validar si está agotado
                    if (productSize.Stock <= 0)
                    {
                        TempData["Error"] = $"No se puede agregar: la Talla {size} de {product.Name} está agotada.";
                        return RedirectToAction("Index");
                    }

                    // 2. RESTRICCIÓN ESTRICTA: Si pide más de lo que hay en stock, NO se registra nada
                    if (quantity > productSize.Stock)
                    {
                        TempData["Error"] = $"No se puede procesar: Solicitaste {quantity} unidades, pero solo hay {productSize.Stock} disponibles en stock para la Talla {size}. Por favor selecciona una cantidad máxima de {productSize.Stock}.";
                        return RedirectToAction("Index");
                    }

                    // 3. Validar con lo que ya tiene en el carrito acumulado
                    var existingInCart = CartService.Instance.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);
                    int yaEnCarrito = existingInCart?.Quantity ?? 0;

                    if (yaEnCarrito + quantity > productSize.Stock)
                    {
                        int disponibles = productSize.Stock - yaEnCarrito;
                        if (disponibles <= 0)
                        {
                            TempData["Error"] = $"No se puede agregar: Ya tienes {yaEnCarrito} en tu carrito y el stock total es de {productSize.Stock} unidad(es).";
                        }
                        else
                        {
                            TempData["Error"] = $"No se puede agregar: Tienes {yaEnCarrito} en el carrito y solo quedan {disponibles} unidad(es) disponibles de {productSize.Stock} en total.";
                        }
                        return RedirectToAction("Index");
                    }

                    // 4. Si la cantidad es válida y dentro del stock, se agrega al carrito
                    CartService.Instance.AddItem(product.Id, product.Name, size, product.ImageUrl, quantity);
                    TempData["Success"] = $"¡{quantity}x {product.Name} (Talla {size}) agregado(s) a la preorden con éxito!";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
