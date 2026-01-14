using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace LegacyOrderService.Data;

public class ProductRepository : IProductRepository
{
        private readonly Dictionary<string, decimal> _productPrices = new()
        {
            ["Widget"] = 12.99m,
            ["Gadget"] = 15.49m,
            ["Doohickey"] = 8.75m
        };
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration;

        public ProductRepository(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            var expirationMinutes = configuration.GetValue<int>("Caching:ProductPriceCacheExpirationMinutes");
            _cacheExpiration = TimeSpan.FromMinutes(expirationMinutes);
        }

        public decimal GetPrice(string productName)
        {
            if (_cache.TryGetValue($"product_price_{productName}", out decimal cachedPrice))
            {
                return cachedPrice;
            }

            // If not in cache, get from dictionary
            if (_productPrices.TryGetValue(productName, out var price))
            {
                // Store in cache for future requests
                _cache.Set($"product_price_{productName}", price, _cacheExpiration);
                return price;
            }

            throw new Exception("Product not found");
        }
    }
