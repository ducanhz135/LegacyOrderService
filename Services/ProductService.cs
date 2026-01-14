using LegacyOrderService.Data;

namespace LegacyOrderService.Services;

public class ProductService : IProductService
{
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public decimal GetProductPrice(string productName)
        {
            return _productRepository.GetPrice(productName);
        }
    }
