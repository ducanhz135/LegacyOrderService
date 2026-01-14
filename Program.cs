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
            .AddSingleton<IOrderProcessing, OrderProcessing>()
            .BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Welcome to Order Processor!");

            // Initialize database
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

            // Process order
            var orderProcessing = serviceProvider.GetRequiredService<IOrderProcessing>();
            await orderProcessing.ProcessOrderAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred");
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}
