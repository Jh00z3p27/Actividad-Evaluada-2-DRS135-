using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApp.Data;
using EcommerceApp.Models;
using EcommerceApp.Services;

namespace EcommerceApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = CartService.Instance;
            return View(cart);
        }

        [HttpPost]
        public IActionResult Remove(int productId, int size)
        {
            CartService.Instance.RemoveItem(productId, size);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            if (!CartService.Instance.Items.Any())
            {
                return RedirectToAction("Index");
            }

            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(Order order)
        {
            var cart = CartService.Instance;
            if (!cart.Items.Any()) return RedirectToAction("Index");

            // 1. RESTRICCIÓN ESTRICTA DE STOCK: No registrar la orden si no hay suficiente stock
            foreach (var item in cart.Items)
            {
                var ps = await _context.ProductSizes
                    .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.SizeNumber == item.Size);

                if (ps == null || ps.Stock < item.Quantity)
                {
                    TempData["Error"] = $"No se puede registrar el pedido: Solo hay {ps?.Stock ?? 0} unidad(es) en stock para {item.ProductName} (Talla {item.Size}) y solicitaste {item.Quantity}.";
                    return RedirectToAction("Index");
                }
            }

            order.Subtotal = cart.GetTotal();
            order.ShippingCost = 0.00m; // Cotización pendiente
            order.Status = OrderStatus.PreordenPendiente;
            order.OrderDate = DateTime.Now;

            foreach (var item in cart.Items)
            {
                var productSize = await _context.ProductSizes
                    .FirstOrDefaultAsync(ps => ps.ProductId == item.ProductId && ps.SizeNumber == item.Size);

                if (productSize != null)
                {
                    productSize.Stock -= item.Quantity; // Descuento de inventario
                }

                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    SelectedSize = item.Size,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            // Registrar o asociar cliente para la Agenda de Clientes
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Email == order.CustomerEmail);

            if (customer == null)
            {
                customer = new Customer
                {
                    Name = order.CustomerName,
                    Email = order.CustomerEmail,
                    Phone = order.CustomerPhone,
                    RegistrationDate = DateTime.Now
                };
                _context.Customers.Add(customer);
            }
            customer.Orders.Add(order);

            await _context.SaveChangesAsync();

            cart.Clear();

            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return RedirectToAction("Index", "Home");

            return View(order);
        }
    }
}
