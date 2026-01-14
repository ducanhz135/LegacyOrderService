using System;
using LegacyOrderService.Models;
using LegacyOrderService.Data;
using LegacyOrderService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace LegacyOrderService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Setup dependency injection
            var serviceProvider = new ServiceCollection()
                .AddMemoryCache()
                .AddSingleton<IOrderRepository, OrderRepository>()
                .AddSingleton<IProductRepository, ProductRepository>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<IProductService, ProductService>()
                .AddSingleton<IInputValidationService, InputValidationService>()
                .BuildServiceProvider();

            try
            {
                Console.WriteLine("Welcome to Order Processor!");

                // Initialize database using DI
                var orderRepo = serviceProvider.GetRequiredService<IOrderRepository>();
                try
                {
                    await orderRepo.InitializeDatabaseAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing database: {ex.Message}");
                    return;
                }

                // Get services
                var validationService = serviceProvider.GetRequiredService<IInputValidationService>();
                var productService = serviceProvider.GetRequiredService<IProductService>();
                var orderService = serviceProvider.GetRequiredService<IOrderService>();

                // Get and validate customer name
                Console.WriteLine("Enter customer name:");
                string customerName = Console.ReadLine() ?? string.Empty;
                if (!validationService.ValidateCustomerName(customerName, out string nameError))
                {
                    Console.WriteLine($"Error: {nameError}");
                    return;
                }

                // Get and validate product name
                Console.WriteLine("Enter product name:");
                string productName = Console.ReadLine() ?? string.Empty;
                if (!validationService.ValidateProductName(productName, out string productError))
                {
                    Console.WriteLine($"Error: {productError}");
                    return;
                }

                // Get product price with error handling
                decimal price;
                try
                {
                    price = productService.GetProductPrice(productName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return;
                }

                // Get and validate quantity
                Console.WriteLine("Enter quantity:");
                string? quantityInput = Console.ReadLine();
                if (!validationService.ValidateQuantity(quantityInput ?? string.Empty, out int quantity, out string quantityError))
                {
                    Console.WriteLine($"Error: {quantityError}");
                    return;
                }

                Console.WriteLine("Processing order...");

                // Create order using service
                Order order = orderService.CreateOrder(customerName, productName, quantity, price);
                decimal total = orderService.CalculateTotal(order);

                Console.WriteLine("Order complete!");
                Console.WriteLine($"Customer: {order.CustomerName}");
                Console.WriteLine($"Product: {order.ProductName}");
                Console.WriteLine($"Quantity: {order.Quantity}");
                Console.WriteLine($"Total: ${total:F2}");

                // Save order with error handling
                Console.WriteLine("Saving order to database...");
                try
                {
                    await orderService.SaveOrderAsync(order);
                    Console.WriteLine("Done.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving order: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
