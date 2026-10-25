using Microsoft.EntityFrameworkCore;
using EcommerceApp.Models;

namespace EcommerceApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cargar Categorías por defecto
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Solitarios" },
                new Category { Id = 2, Name = "Anillos de Promesa" },
                new Category { Id = 3, Name = "Colecciones" }
            );

            // Cargar Productos por defecto
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Anillo Solitario Corazón",
                    Description = "Anillo estilo Pandora en Plata 925 con circón en forma de corazón. Incluye cajita de la marca.",
                    Price = 35.00m,
                    CategoryId = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1605100804763-247f67b3557e?w=500"
                },
                new Product
                {
                    Id = 2,
                    Name = "Anillo Promesa Infinito",
                    Description = "Anillo de promesa con grabado de infinito y cristales laterales. Incluye cajita de la marca.",
                    Price = 35.00m,
                    CategoryId = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1603561591411-07134e71a2a9?w=500"
                },
                new Product
                {
                    Id = 3,
                    Name = "Anillo Colección Corona Real",
                    Description = "Edición especial estilo corona con circones montados a mano. Incluye cajita de la marca.",
                    Price = 35.00m,
                    CategoryId = 3,
                    ImageUrl = "https://images.unsplash.com/photo-1598560917505-59a3ad559071?w=500"
                }
            );

            // Cargar Tallas Iniciales
            modelBuilder.Entity<ProductSize>().HasData(
                new ProductSize { Id = 1, ProductId = 1, SizeNumber = 5, Stock = 2 },
                new ProductSize { Id = 2, ProductId = 1, SizeNumber = 6, Stock = 5 },
                new ProductSize { Id = 3, ProductId = 1, SizeNumber = 7, Stock = 4 },
                new ProductSize { Id = 4, ProductId = 1, SizeNumber = 8, Stock = 2 },
                new ProductSize { Id = 5, ProductId = 2, SizeNumber = 6, Stock = 6 },
                new ProductSize { Id = 6, ProductId = 2, SizeNumber = 7, Stock = 2 },
                new ProductSize { Id = 7, ProductId = 2, SizeNumber = 8, Stock = 5 },
                new ProductSize { Id = 8, ProductId = 3, SizeNumber = 5, Stock = 2 },
                new ProductSize { Id = 9, ProductId = 3, SizeNumber = 6, Stock = 3 },
                new ProductSize { Id = 10, ProductId = 3, SizeNumber = 7, Stock = 3 }
            );
        }
    }
}