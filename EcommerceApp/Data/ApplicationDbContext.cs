using Microsoft.EntityFrameworkCore;
using EcommerceApp.Models;

namespace EcommerceApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductSize> ProductSizes { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Categorías Iniciales de Brillé
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Anillos de Promesa" },
                new Category { Id = 2, Name = "Colecciones" },
                new Category { Id = 3, Name = "Solitarios" }
            );

            // 2. Anillos Iniciales
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Anillo Solitario Corazón", Description = "Anillo estilo Pandora en Plata 925 con circón en forma de corazón. Incluye cajita de la marca.", Price = 35.00m, CategoryId = 3, ImageUrl = "https://via.placeholder.com/300x300?text=Solitario+Corazon" },
                new Product { Id = 2, Name = "Anillo Promesa Infinito", Description = "Anillo de promesa con grabado de infinito y cristales laterales. Incluye cajita de la marca.", Price = 35.00m, CategoryId = 1, ImageUrl = "https://via.placeholder.com/300x300?text=Promesa+Infinito" },
                new Product { Id = 3, Name = "Anillo Colección Corona Real", Description = "Edición especial estilo corona con circones montados a mano. Incluye cajita de la marca.", Price = 35.00m, CategoryId = 2, ImageUrl = "https://via.placeholder.com/300x300?text=Corona+Real" }
            );

            // 3. Tallas y Stock Específico para cada Anillo (Tallas 5 a 10)
            modelBuilder.Entity<ProductSize>().HasData(
                // Anillo 1 (Solitario Corazón)
                new ProductSize { Id = 1, ProductId = 1, SizeNumber = 5, Stock = 3 },
                new ProductSize { Id = 2, ProductId = 1, SizeNumber = 6, Stock = 5 },
                new ProductSize { Id = 3, ProductId = 1, SizeNumber = 7, Stock = 4 },
                new ProductSize { Id = 4, ProductId = 1, SizeNumber = 8, Stock = 2 },

                // Anillo 2 (Promesa Infinito)
                new ProductSize { Id = 5, ProductId = 2, SizeNumber = 6, Stock = 6 },
                new ProductSize { Id = 6, ProductId = 2, SizeNumber = 7, Stock = 3 },
                new ProductSize { Id = 7, ProductId = 2, SizeNumber = 8, Stock = 5 },

                // Anillo 3 (Corona Real)
                new ProductSize { Id = 8, ProductId = 3, SizeNumber = 5, Stock = 2 },
                new ProductSize { Id = 9, ProductId = 3, SizeNumber = 6, Stock = 4 },
                new ProductSize { Id = 10, ProductId = 3, SizeNumber = 7, Stock = 3 }
            );
        }
    }
}