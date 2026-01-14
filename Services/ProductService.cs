using LegacyOrderService.Data;
using Microsoft.Extensions.Logging;

namespace LegacyOrderService.Services;

public class ProductService : IProductService
{
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public decimal GetProductPrice(string productName)
        {
            _logger.LogDebug("Retrieving price for product: {ProductName}", productName);
            var price = _productRepository.GetPrice(productName);
            _logger.LogDebug("Price for {ProductName}: {Price:C}", productName, price);
            return price;
        }
    }
