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

            _context.Orders.Add(order);
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