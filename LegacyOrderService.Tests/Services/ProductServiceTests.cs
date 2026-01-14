using Xunit;
using LegacyOrderService.Data;
using LegacyOrderService.Services;
using Moq;
using Microsoft.Extensions.Logging;

namespace LegacyOrderService.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _productService = new ProductService(_mockProductRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void GetProductPrice_ShouldReturnPriceFromRepository()
    {
        // Arrange
        string productName = "Widget";
        decimal expectedPrice = 12.99m;
        _mockProductRepository.Setup(r => r.GetPrice(productName)).Returns(expectedPrice);

        // Act
        decimal actualPrice = _productService.GetProductPrice(productName);

        // Assert
        Assert.Equal(expectedPrice, actualPrice);
        _mockProductRepository.Verify(r => r.GetPrice(productName), Times.Once);
    }

    [Theory]
    [InlineData("Widget", 12.99)]
    [InlineData("Gadget", 15.49)]
    [InlineData("Doohickey", 8.75)]
    public void GetProductPrice_ForKnownProducts_ShouldReturnCorrectPrice(string productName, decimal expectedPrice)
    {
        _mockProductRepository.Setup(r => r.GetPrice(productName)).Returns(expectedPrice);
        decimal actualPrice = _productService.GetProductPrice(productName);

        Assert.Equal(expectedPrice, actualPrice);
    }

    [Fact]
    public void GetProductPrice_ForUnknownProduct_ShouldThrowException()
    {
        string productName = "UnknownProduct";
        _mockProductRepository.Setup(r => r.GetPrice(productName))
            .Throws(new Exception("Product not found"));

        var exception = Assert.Throws<Exception>(() => _productService.GetProductPrice(productName));
        Assert.Equal("Product not found", exception.Message);
    }

    [Fact]
    public void GetProductPrice_WithNullProductName_ShouldThrowException()
    {
        _mockProductRepository.Setup(r => r.GetPrice(null!))
            .Throws(new ArgumentNullException("productName"));

        Assert.Throws<ArgumentNullException>(() => _productService.GetProductPrice(null!));
    }
}
