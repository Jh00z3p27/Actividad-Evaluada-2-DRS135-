using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{
    public enum ShippingMethod
    {
        Encomienda, // Pedidos Express ($2.00)
        EntregaPersonalizada // Puntos acordados en Santa Ana
    }

    public enum PaymentMethod
    {
        Efectivo,
        TransferenciaBancaria
    }

    public enum OrderStatus
    {
        PreordenPendiente,
        EsperandoComprobante,
        ValidadoYConfirmado,
        EnEmpaque,
        Enviado,
        Completado,
        Devuelto,
        Cancelado
    }

    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio"), EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "El WhatsApp/Teléfono es obligatorio")]
        public string CustomerPhone { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;
        public string Municipality { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección de entrega o punto es obligatoria")]
        public string DestinationAddress { get; set; } = string.Empty;

        public ShippingMethod Shipping { get; set; } = ShippingMethod.Encomienda;
        public PaymentMethod Payment { get; set; } = PaymentMethod.Efectivo;

        public decimal ShippingCost { get; set; } = 0.00m;
        public decimal Subtotal { get; set; }
        public decimal Total => Subtotal + ShippingCost;

        public OrderStatus Status { get; set; } = OrderStatus.PreordenPendiente;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime PreferredDeliveryDate { get; set; } = DateTime.Now.AddDays(2);

        public bool PaymentProofReceived { get; set; } = false;

        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int SelectedSize { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}