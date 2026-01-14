using LegacyOrderService.Data;
using LegacyOrderService.Models;
using Microsoft.Extensions.Logging;

namespace LegacyOrderService.Services;

public class OrderService : IOrderService
{
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public Order CreateOrder(string customerName, string productName, int quantity, decimal price)
        {
            _logger.LogDebug("Creating order for customer: {CustomerName}", customerName);
            return new Order
            {
                Id = Guid.NewGuid().ToString(),
                CustomerName = customerName,
                ProductName = productName,
                Quantity = quantity,
                Price = price
            };
        }

        public decimal CalculateTotal(Order order)
        {
            var total = order.Quantity * order.Price;
            _logger.LogDebug("Calculated total for order {OrderId}: {Total:C}", order.Id, total);
            return total;
        }

        public async Task SaveOrderAsync(Order order)
        {
            _logger.LogInformation("Saving order {OrderId} to repository", order.Id);
            await _orderRepository.SaveAsync(order);
            _logger.LogInformation("Order {OrderId} saved successfully", order.Id);
        }
    }
