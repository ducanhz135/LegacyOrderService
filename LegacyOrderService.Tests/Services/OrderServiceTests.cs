using Xunit;
using LegacyOrderService.Data;
using LegacyOrderService.Models;
using LegacyOrderService.Services;
using Moq;

namespace LegacyOrderService.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _orderService = new OrderService(_mockOrderRepository.Object);
    }

    [Fact]
    public void CreateOrder_ShouldReturnOrderWithCorrectProperties()
    {
        // Arrange
        string customerName = "Mr.A";
        string productName = "Widget";
        int quantity = 5;
        decimal price = 12.99m;

        // Act
        var order = _orderService.CreateOrder(customerName, productName, quantity, price);

        // Assert
        Assert.NotNull(order);
        Assert.NotEmpty(order.Id);
        Assert.Equal(customerName, order.CustomerName);
        Assert.Equal(productName, order.ProductName);
        Assert.Equal(quantity, order.Quantity);
        Assert.Equal(price, order.Price);
    }

    [Fact]
    public void CreateOrder_ShouldGenerateUniqueIds()
    {
        string customerName = "Mr.A";
        string productName = "Widget";
        int quantity = 5;
        decimal price = 12.99m;

        var order1 = _orderService.CreateOrder(customerName, productName, quantity, price);
        var order2 = _orderService.CreateOrder(customerName, productName, quantity, price);

        Assert.NotEqual(order1.Id, order2.Id);
    }

    [Fact]
    public void CalculateTotal_ShouldReturnCorrectTotal()
    {
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerName = "Mr.A",
            ProductName = "Widget",
            Quantity = 5,
            Price = 12.99m
        };

        decimal total = _orderService.CalculateTotal(order);

        Assert.Equal(64.95m, total);
    }

    [Fact]
    public void CalculateTotal_WithZeroQuantity_ShouldReturnZero()
    {
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerName = "Mr.A",
            ProductName = "Widget",
            Quantity = 0,
            Price = 12.99m
        };

        decimal total = _orderService.CalculateTotal(order);

        Assert.Equal(0m, total);
    }

    [Fact]
    public async Task SaveOrderAsync_ShouldCallRepositorySaveAsync()
    {
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerName = "Mr.A",
            ProductName = "Widget",
            Quantity = 5,
            Price = 12.99m
        };

        await _orderService.SaveOrderAsync(order);

        _mockOrderRepository.Verify(r => r.SaveAsync(order), Times.Once);
    }

    [Theory]
    [InlineData(1, 10.0, 10.0)]
    [InlineData(2, 5.5, 11.0)]
    [InlineData(10, 1.99, 19.9)]
    [InlineData(100, 0.5, 50.0)]
    public void CalculateTotal_WithVariousInputs_ShouldReturnCorrectTotal(int quantity, decimal price, decimal expectedTotal)
    {
        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            CustomerName = "Test Customer",
            ProductName = "Test Product",
            Quantity = quantity,
            Price = price
        };

        decimal total = _orderService.CalculateTotal(order);

        Assert.Equal(expectedTotal, total);
    }
}
