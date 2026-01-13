using System;
using LegacyOrderService.Models;
using LegacyOrderService.Data;
using Microsoft.Extensions.DependencyInjection;

namespace LegacyOrderService
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup dependency injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IOrderRepository, OrderRepository>()
                .AddSingleton<IProductRepository, ProductRepository>()
                .BuildServiceProvider();

            try
            {
                Console.WriteLine("Welcome to Order Processor!");

                // Initialize database using DI
                var orderRepo = serviceProvider.GetRequiredService<IOrderRepository>();
                try
                {
                    orderRepo.InitializeDatabase();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing database: {ex.Message}");
                    return;
                }

                // Get and validate customer name
                Console.WriteLine("Enter customer name:");
                string name = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Error: Customer name cannot be empty.");
                    return;
                }

                // Get and validate product name
                Console.WriteLine("Enter product name:");
                string product = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(product))
                {
                    Console.WriteLine("Error: Product name cannot be empty.");
                    return;
                }

                // Get product price with error handling using DI
                var productRepo = serviceProvider.GetRequiredService<IProductRepository>();
                double price;
                try
                {
                    price = productRepo.GetPrice(product);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return;
                }

                // Get and validate quantity
                Console.WriteLine("Enter quantity:");
                int qty;
                string? qtyInput = Console.ReadLine();
                if (!int.TryParse(qtyInput, out qty) || qty <= 0)
                {
                    Console.WriteLine("Error: Quantity must be a positive number.");
                    return;
                }

                Console.WriteLine("Processing order...");

                Order order = new Order();
                order.CustomerName = name;
                order.ProductName = product;
                order.Quantity = qty;
                order.Price = price;

                double total = order.Quantity * order.Price;

                Console.WriteLine("Order complete!");
                Console.WriteLine("Customer: " + order.CustomerName);
                Console.WriteLine("Product: " + order.ProductName);
                Console.WriteLine("Quantity: " + order.Quantity);
                Console.WriteLine("Total: $" + total);

                // Save order with error handling
                Console.WriteLine("Saving order to database...");
                try
                {
                    orderRepo.Save(order);
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
