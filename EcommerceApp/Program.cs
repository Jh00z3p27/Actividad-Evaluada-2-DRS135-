using Microsoft.EntityFrameworkCore;
using EcommerceApp.Data;
using EcommerceApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=brille.db"));

// Registro del Patrón Singleton para el Carrito de Compras
builder.Services.AddSingleton(CartService.Instance);

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Inicializar la base de datos automáticamente si no existe
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();