using EcommerceApp.Models;

namespace EcommerceApp.Services
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Size { get; set; }
        public decimal Price { get; set; } = 35.00m;
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Subtotal => Price * Quantity;
    }

    public class CartService
    {
        private static CartService? _instance;
        private static readonly object _lock = new object();

        public List<CartItem> Items { get; private set; } = new();

        private CartService() { }

        // Aplicación del Patrón Singleton
        public static CartService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CartService();
                    }
                    return _instance;
                }
            }
        }

        public void AddItem(int productId, string name, int size, string imageUrl, int quantity = 1)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                Items.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = name,
                    Size = size,
                    Price = 35.00m,
                    Quantity = quantity,
                    ImageUrl = imageUrl
                });
            }
        }

        public void RemoveItem(int productId, int size)
        {
            Items.RemoveAll(i => i.ProductId == productId && i.Size == size);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public decimal GetTotal() => Items.Sum(i => i.Subtotal);
    }
}