# ✨ Brillé Joyería - E-Commerce System (DRS-135) 🌸

Bienvenido al repositorio oficial del proyecto **"Brillé"**, una aplicación Web E-Commerce desarrollada en C# ASP.NET Core MVC para la asignatura **Desarrollo y Reutilización de Software (DRS-135)** en la **Universidad de El Salvador**.

---

## 👥 Equipo de Desarrollo (Grupo #5)
* 💖 **Jose Cerritos** (Líder Técnico & Arquitectura)
* 🌸 **Elizabeth Hernandez**
* 🛠️ **Miguel Linares**
* 📦 **Jose Mena**
* 🚀 **Wilian Salinas**

---

## 🛠️ Tecnologías y Arquitectura Utilizada
* **Framework:** .NET 8.0 ASP.NET Core MVC
* **Lenguaje:** C#
* **Base de Datos:** SQLite con Entity Framework Core 8.0
* **Frontend:** Razor Views, Bootstrap 5 & CSS3 Personalizado
* **Control de Versiones:** Git & GitHub

---

## 📐 Patrones de Diseño Implementados

1. **Modelo-Vista-Controlador (MVC):** Separación modular entre los modelos de datos (`Product`, `Order`, `Customer`), las vistas Razor de la joyería y los controladores de lógica (`HomeController`, `CartController`, `AdminController`).
2. **Patrón Singleton (`CartService`):** Garantiza una instancia única y global del carrito de compras en memoria para la gestión de productos, selección de tallas y subtotales.
3. **Patrón Observer (`InventoryObserver`):** Sistema de notificaciones reactivo que restaura automáticamente el stock en SQLite cuando una preorden cambia a estado *Devuelto* o *Cancelado*.

---

## 🚚 Flujo de Logística y Reservas de Brillé
* **Preordenes Inteligentes:** Selección de anillos por tallas (5 a 10) con confirmación directa por WhatsApp y Correo.
* **Logística de Envíos:** Encomiendas con Pedidos Express ($2.00) y Entregas Personalizadas en puntos estratégicos de Santa Ana.
* **Términos de Pago:** Efectivo contra entrega y Transferencia Bancaria previa con validación de comprobante en un máximo de 24 horas.

---

> 💖 *Brillé Joyería - Donde cada brillo cuenta.* ✨Transferencia Bancaria Previa con validacion de comprobante en un maximo de 24 horas.
