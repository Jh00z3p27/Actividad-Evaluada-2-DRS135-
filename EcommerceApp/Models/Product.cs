namespace EcommerceApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ej: "Anillo Solitario Corazón"
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 35.00m; // Precio base $35.00
        public string ImageUrl { get; set; } = string.Empty;

        // Relación con Categoría (Anillos de Promesa, Colecciones, Solitarios)
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Variantes de Talla y Stock Específico
        public List<ProductSize> Sizes { get; set; } = new();
    }

    public class ProductSize
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int SizeNumber { get; set; } // Tallas de 5 a 10
        public int Stock { get; set; } // Stock específico por talla
    }
}