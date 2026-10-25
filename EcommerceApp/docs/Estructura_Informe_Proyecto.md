# 📄 Estructura del Informe de Proyecto - Brillé Joyería (DRS-135)

## 1. Introducción y Justificación
El presente proyecto aborda el desarrollo de un sistema E-Commerce especializado en la comercialización de anillos ("Brillé Joyería"), aplicando principios de reutilización de software y patrones de diseño orientados a objetos.

## 2. Análisis de Patrones Utilizados
* **Arquitectura MVC (ASP.NET Core):** Permite la separación de responsabilidades entre los datos, vistas y lógica de control.
* **Patrón Singleton:** Implementado en `CartService` para preservar el estado del carrito en memoria de forma centralizada.
* **Patrón Observer:** Implementado en `InventoryObserver` para la restitución automatizada de existencias tras cancelaciones o devoluciones.

## 3. Conclusiones y Trabajo Futuro
La modularidad implementada garantiza la escalabilidad del sistema para futuras expansiones en el área de joyería fina.
