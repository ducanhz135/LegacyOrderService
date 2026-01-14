using LegacyOrderService.Models;
using LegacyOrderService.Data;
using LegacyOrderService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LegacyOrderService;

class Program
{
    static async Task Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Setup dependency injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .AddMemoryCache()
                .AddSingleton<IOrderRepository, OrderRepository>()
                .AddSingleton<IProductRepository, ProductRepository>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<IProductService, ProductService>()
                .AddSingleton<IInputValidationService, InputValidationService>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Welcome to Order Processor!");

                // Initialize database using DI
                var orderRepo = serviceProvider.GetRequiredService<IOrderRepository>();
                try
                {
                    await orderRepo.InitializeDatabaseAsync();
                    logger.LogInformation("Database initialized successfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error initializing database");
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
                    logger.LogWarning("Customer name validation failed: {Error}", nameError);
                    Console.WriteLine($"Error: {nameError}");
                    return;
                }

                // Get and validate product name
                Console.WriteLine("Enter product name:");
                string productName = Console.ReadLine() ?? string.Empty;
                if (!validationService.ValidateProductName(productName, out string productError))
                {
                    logger.LogWarning("Product name validation failed: {Error}", productError);
                    Console.WriteLine($"Error: {productError}");
                    return;
                }

                // Get product price with error handling
                decimal price;
                try
                {
                    price = productService.GetProductPrice(productName);
                    logger.LogInformation("Retrieved price for {ProductName}: {Price:C}", productName, price);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error retrieving product price for {ProductName}", productName);
                    Console.WriteLine($"Error: {ex.Message}");
                    return;
                }

                // Get and validate quantity
                Console.WriteLine("Enter quantity:");
                string? quantityInput = Console.ReadLine();
                if (!validationService.ValidateQuantity(quantityInput ?? string.Empty, out int quantity, out string quantityError))
                {
                    logger.LogWarning("Quantity validation failed: {Error}", quantityError);
                    Console.WriteLine($"Error: {quantityError}");
                    return;
                }

                logger.LogInformation("Processing order for {CustomerName}: {Quantity} x {ProductName}", customerName, quantity, productName);

                // Create order using service
                Order order = orderService.CreateOrder(customerName, productName, quantity, price);
                decimal total = orderService.CalculateTotal(order);

                logger.LogInformation("Order created: {OrderId}", order.Id);
                Console.WriteLine("Order complete!");
                Console.WriteLine($"Customer: {order.CustomerName}");
                Console.WriteLine($"Product: {order.ProductName}");
                Console.WriteLine($"Quantity: {order.Quantity}");
                Console.WriteLine($"Total: ${total:F2}");

                // Save order with error handling
                logger.LogInformation("Saving order {OrderId} to database", order.Id);
                try
                {
                    await orderService.SaveOrderAsync(order);
                    logger.LogInformation("Order {OrderId} saved successfully", order.Id);
                    Console.WriteLine("Done.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error saving order {OrderId}", order.Id);
                    Console.WriteLine($"Error saving order: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred");
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
