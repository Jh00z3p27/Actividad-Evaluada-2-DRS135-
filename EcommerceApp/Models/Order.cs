namespace EcommerceApp.Models
{
    public enum ShippingMethod
    {
        MeloExpress,     // $1.50 (Lunes y Jueves)
        PedidosExpress,  // $2.00 (Martes y Viernes)
        EntregaPersonal  // Costo personalizado
    }

    public enum OrderStatus
    {
        Pendiente,
        Confirmado,
        Empaquetado,
        Enviado,
        Completado,
        Devuelto,  // Al cambiar a Devuelto, el sistema suma +1 al stock de la talla
        Cancelado
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string DestinationAddress { get; set; } = string.Empty;
        
        public ShippingMethod Shipping { get; set; }
        public decimal ShippingCost { get; set; }
        
        public DateTime PreferredDeliveryDate { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        public OrderStatus Status { get; set; } = OrderStatus.Pendiente;
        public decimal Total { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int SelectedSize { get; set; } // Talla elegida (5 a 10)
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}