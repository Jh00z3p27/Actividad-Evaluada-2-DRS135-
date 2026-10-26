using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApp.Data;
using EcommerceApp.Models;
using EcommerceApp.Services;

namespace EcommerceApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private bool IsAdminAuthenticated()
        {
            return HttpContext.Session.GetString("IsAdmin") == "true";
        }

        public IActionResult Login()
        {
            if (IsAdminAuthenticated()) return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public IActionResult Login(string password)
        {
            if (password == "Brille2026")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Contraseña incorrecta. Intente de nuevo.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Punto 1: Inventario de Anillos
        public async Task<IActionResult> Index()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Sizes)
                .ToListAsync();

            return View(products);
        }

        // Punto 1 y 2: Formulario Crear Anillo & Categorías
        public async Task<IActionResult> Create()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                _context.Categories.Add(new Category { Name = categoryName.Trim() });
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Categoría '{categoryName}' creada exitosamente.";
            }
            return RedirectToAction("Create");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
                if (hasProducts)
                {
                    TempData["Error"] = "No se puede eliminar la categoría porque tiene productos vinculados.";
                }
                else
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Categoría eliminada correctamente.";
                }
            }
            return RedirectToAction("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile, string sizesStock)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                product.ImageUrl = "/uploads/" + uniqueFileName;
            }
            else
            {
                product.ImageUrl = "https://images.unsplash.com/photo-1605100804763-247f67b3557e?w=500";
            }

            product.Price = 35.00m;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(sizesStock))
            {
                var pairs = sizesStock.Split(',');
                foreach (var pair in pairs)
                {
                    var parts = pair.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int size) && int.TryParse(parts[1].Trim(), out int stock))
                    {
                        _context.ProductSizes.Add(new ProductSize
                        {
                            ProductId = product.Id,
                            SizeNumber = size,
                            Stock = stock
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "¡Anillo guardado exitosamente en el inventario!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var product = await _context.Products.Include(p => p.Sizes).FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                _context.ProductSizes.RemoveRange(product.Sizes);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Anillo borrado del inventario.";
            }
            return RedirectToAction("Index");
        }

        // Punto 4 y 5: Gestión de Preordenes, Encomiendas y Comprobantes
        public async Task<IActionResult> Orders()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderDetails(int orderId, decimal shippingCost, OrderStatus newStatus, bool paymentProofReceived)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                var previousStatus = order.Status;

                // Determinar si debe restaurarse el stock ANTES de cambiar el estado
                bool restaurarStock = (newStatus == OrderStatus.Cancelado || newStatus == OrderStatus.Devuelto)
                                   && (previousStatus != OrderStatus.Cancelado && previousStatus != OrderStatus.Devuelto);

                // Restaurar stock directamente en la BD
                if (restaurarStock)
                {
                    foreach (var item in order.Items)
                    {
                        var productSize = await _context.ProductSizes
                            .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.SizeNumber == item.SelectedSize);

                        if (productSize != null)
                        {
                            productSize.Stock += item.Quantity;
                        }
                    }
                }

                // Actualizar datos de la orden
                order.ShippingCost = shippingCost;
                order.Status = newStatus;
                order.PaymentProofReceived = paymentProofReceived;

                // Patrón Observer: notificación de cambio de estado
                var subject = new OrderSubject();
                subject.Attach(new NotificationObserver());
                await subject.NotifyAsync(order, previousStatus, _context);

                await _context.SaveChangesAsync();

                string mensajeExtra = restaurarStock ? " — ¡Stock devuelto al inventario automáticamente!" : "";
                TempData["Success"] = $"Preorden #{order.Id} actualizada: Estado: {newStatus}{mensajeExtra}.";
            }
            return RedirectToAction("Orders");
        }

        // Punto 3 y 7: Agenda de Clientes & Control de Baneos
        public async Task<IActionResult> Customers()
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var customers = await _context.Customers
                .Include(c => c.Orders)
                .ToListAsync();

            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBanCustomer(int id, string banReason)
        {
            if (!IsAdminAuthenticated()) return RedirectToAction("Login");

            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.IsBanned = !customer.IsBanned;
                customer.BanReason = customer.IsBanned ? (string.IsNullOrEmpty(banReason) ? "Inasistencia o no retiro de paquete" : banReason) : string.Empty;
                customer.BanUntil = customer.IsBanned ? DateTime.Now.AddDays(30) : null;

                await _context.SaveChangesAsync();
                TempData["Success"] = customer.IsBanned ? $"Cliente {customer.Name} suspendido." : $"Cliente {customer.Name} reactivado.";
            }

            return RedirectToAction("Customers");
        }
    }
}
