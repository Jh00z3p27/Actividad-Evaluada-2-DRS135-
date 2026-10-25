using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Services
{
    public interface IOrderObserver
    {
        Task UpdateAsync(Order order, OrderStatus previousStatus, ApplicationDbContext context);
    }

    public class StockRestorationObserver : IOrderObserver
    {
        public async Task UpdateAsync(Order order, OrderStatus previousStatus, ApplicationDbContext context)
        {
            // Si la orden cambia a Devuelto o Cancelado, el Patrón Observer restaura automáticamente el stock en SQLite
            if (order.Status == OrderStatus.Devuelto || order.Status == OrderStatus.Cancelado)
            {
                if (previousStatus != OrderStatus.Devuelto && previousStatus != OrderStatus.Cancelado)
                {
                    foreach (var item in order.Items)
                    {
                        var size = await context.ProductSizes
                            .FirstOrDefaultAsync(s => s.ProductId == item.ProductId && s.SizeNumber == item.SelectedSize);

                        if (size != null)
                        {
                            size.Stock += item.Quantity;
                        }
                    }
                }
            }
        }
    }

    public class NotificationObserver : IOrderObserver
    {
        public Task UpdateAsync(Order order, OrderStatus previousStatus, ApplicationDbContext context)
        {
            // Notificación lógica simulada de cambio de estado
            Console.WriteLine($"[NOTIFICACIÓN BRILLE] Preorden #{order.Id} cambió de estado {previousStatus} a {order.Status}.");
            return Task.CompletedTask;
        }
    }

    public class OrderSubject
    {
        private readonly List<IOrderObserver> _observers = new();

        public void Attach(IOrderObserver observer)
        {
            _observers.Add(observer);
        }

        public async Task NotifyAsync(Order order, OrderStatus previousStatus, ApplicationDbContext context)
        {
            foreach (var observer in _observers)
            {
                await observer.UpdateAsync(order, previousStatus, context);
            }
        }
    }
}