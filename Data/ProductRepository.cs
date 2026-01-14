// Data/ProductRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;

namespace LegacyOrderService.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly Dictionary<string, decimal> _productPrices = new()
        {
            ["Widget"] = 12.99m,
            ["Gadget"] = 15.49m,
            ["Doohickey"] = 8.75m
        };

        public decimal GetPrice(string productName)
        {
            if (_productPrices.TryGetValue(productName, out var price))
                return price;

            throw new Exception("Product not found");
        }
    }
}
