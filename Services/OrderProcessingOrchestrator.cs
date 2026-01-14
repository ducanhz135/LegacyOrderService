using LegacyOrderService.Models;
using Microsoft.Extensions.Logging;

namespace LegacyOrderService.Services;

public class OrderProcessing : IOrderProcessing
{
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IInputValidationService _validationService;
    private readonly ILogger<OrderProcessing> _logger;

    public OrderProcessing(
        IOrderService orderService,
        IProductService productService,
        IInputValidationService validationService,
        ILogger<OrderProcessing> logger)
    {
        _orderService = orderService;
        _productService = productService;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task ProcessOrderAsync()
    {
        _logger.LogInformation("Starting order processing");

        // Get and validate customer name
        Console.WriteLine("Enter customer name:");
        string customerName = Console.ReadLine() ?? string.Empty;
        if (!_validationService.ValidateCustomerName(customerName, out string nameError))
        {
            _logger.LogWarning("Customer name validation failed: {Error}", nameError);
            Console.WriteLine($"Error: {nameError}");
            return;
        }

        // Get and validate product name
        Console.WriteLine("Enter product name:");
        string productName = Console.ReadLine() ?? string.Empty;
        if (!_validationService.ValidateProductName(productName, out string productError))
        {
            _logger.LogWarning("Product name validation failed: {Error}", productError);
            Console.WriteLine($"Error: {productError}");
            return;
        }

        // Get product price with error handling
        decimal price;
        try
        {
            price = _productService.GetProductPrice(productName);
            _logger.LogInformation("Retrieved price for {ProductName}: {Price:C}", productName, price);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product price for {ProductName}", productName);
            Console.WriteLine($"Error: {ex.Message}");
            return;
        }

        // Get and validate quantity
        Console.WriteLine("Enter quantity:");
        string? quantityInput = Console.ReadLine();
        if (!_validationService.ValidateQuantity(quantityInput ?? string.Empty, out int quantity, out string quantityError))
        {
            _logger.LogWarning("Quantity validation failed: {Error}", quantityError);
            Console.WriteLine($"Error: {quantityError}");
            return;
        }

        _logger.LogInformation("Processing order for {CustomerName}: {Quantity} x {ProductName}", customerName, quantity, productName);

        // Create order using service
        Order order = _orderService.CreateOrder(customerName, productName, quantity, price);
        decimal total = _orderService.CalculateTotal(order);

        _logger.LogInformation("Order created: {OrderId}", order.Id);
        Console.WriteLine("Order complete!");
        Console.WriteLine($"Customer: {order.CustomerName}");
        Console.WriteLine($"Product: {order.ProductName}");
        Console.WriteLine($"Quantity: {order.Quantity}");
        Console.WriteLine($"Total: ${total:F2}");

        // Save order with error handling
        _logger.LogInformation("Saving order {OrderId} to database", order.Id);
        try
        {
            await _orderService.SaveOrderAsync(order);
            _logger.LogInformation("Order {OrderId} saved successfully", order.Id);
            Console.WriteLine("Done.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving order {OrderId}", order.Id);
            Console.WriteLine($"Error saving order: {ex.Message}");
        }
    }
}
