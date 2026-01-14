using LegacyOrderService.Data;
using LegacyOrderService.Models;

namespace LegacyOrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Order CreateOrder(string customerName, string productName, int quantity, decimal price)
        {
            return new Order
            {
                CustomerName = customerName,
                ProductName = productName,
                Quantity = quantity,
                Price = price
            };
        }

        public decimal CalculateTotal(Order order)
        {
            return order.Quantity * order.Price;
        }

        public async Task SaveOrderAsync(Order order)
        {
            await _orderRepository.SaveAsync(order);
        }
    }
}
