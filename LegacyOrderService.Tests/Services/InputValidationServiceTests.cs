using Xunit;
using LegacyOrderService.Services;

namespace LegacyOrderService.Tests.Services;

public class InputValidationServiceTests
{
    private readonly InputValidationService _validationService;

    public InputValidationServiceTests()
    {
        _validationService = new InputValidationService();
    }


    [Fact]
    public void ValidateCustomerName_WithValidName_ShouldReturnTrue()
    {
        // Arrange
        string validName = "Mr.A";

        // Act
        bool result = _validationService.ValidateCustomerName(validName, out string errorMessage);

        // Assert
        Assert.True(result);
        Assert.Empty(errorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void ValidateCustomerName_WithEmptyOrWhitespace_ShouldReturnFalse(string invalidName)
    {
        // Act
        bool result = _validationService.ValidateCustomerName(invalidName, out string errorMessage);

        // Assert
        Assert.False(result);
        Assert.Equal("Customer name cannot be empty.", errorMessage);
    }

    [Fact]
    public void ValidateCustomerName_WithNull_ShouldReturnFalse()
    {
        // Act
        bool result = _validationService.ValidateCustomerName(null!, out string errorMessage);

        // Assert
        Assert.False(result);
        Assert.Equal("Customer name cannot be empty.", errorMessage);
    }

    [Fact]
    public void ValidateProductName_WithValidName_ShouldReturnTrue()
    {
        // Arrange
        string validProductName = "Widget";

        // Act
        bool result = _validationService.ValidateProductName(validProductName, out string errorMessage);

        // Assert
        Assert.True(result);
        Assert.Empty(errorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void ValidateProductName_WithEmptyOrWhitespace_ShouldReturnFalse(string invalidProductName)
    {
        // Act
        bool result = _validationService.ValidateProductName(invalidProductName, out string errorMessage);

        // Assert
        Assert.False(result);
        Assert.Equal("Product name cannot be empty.", errorMessage);
    }

    [Fact]
    public void ValidateProductName_WithNull_ShouldReturnFalse()
    {
        // Act
        bool result = _validationService.ValidateProductName(null!, out string errorMessage);

        // Assert
        Assert.False(result);
        Assert.Equal("Product name cannot be empty.", errorMessage);
    }


    [Theory]
    [InlineData("1", 1)]
    [InlineData("5", 5)]
    [InlineData("100", 100)]
    [InlineData("999999", 999999)]
    public void ValidateQuantity_WithValidQuantity_ShouldReturnTrue(string input, int expectedQuantity)
    {
        // Act
        bool result = _validationService.ValidateQuantity(input, out int quantity, out string errorMessage);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedQuantity, quantity);
        Assert.Empty(errorMessage);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-100")]
    public void ValidateQuantity_WithZeroOrNegative_ShouldReturnFalse(string input)
    {
        // Act
        bool result = _validationService.ValidateQuantity(input, out int quantity, out string errorMessage);

        // Assert
        Assert.False(result);
        Assert.Equal("Quantity must be a positive number.", errorMessage);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12.5")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1a")]
    public void ValidateQuantity_WithInvalidFormat_ShouldReturnFalse(string input)
    {
        // Act
        bool result = _validationService.ValidateQuantity(input, out int quantity, out string errorMessage);

        // Assert
        Assert.False(result);
        Assert.Equal("Quantity must be a positive number.", errorMessage);
    }

    [Fact]
    public void ValidateQuantity_WithNull_ShouldReturnFalse()
    {
        // Act
        bool result = _validationService.ValidateQuantity(null!, out int quantity, out string errorMessage);

        // Assert
        Assert.False(result);
        Assert.Equal("Quantity must be a positive number.", errorMessage);
    }

    
}
