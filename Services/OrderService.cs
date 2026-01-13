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

        public Order CreateOrder(string customerName, string productName, int quantity, double price)
        {
            return new Order
            {
                CustomerName = customerName,
                ProductName = productName,
                Quantity = quantity,
                Price = price
            };
        }

        public double CalculateTotal(Order order)
        {
            return order.Quantity * order.Price;
        }

        public void SaveOrder(Order order)
        {
            _orderRepository.Save(order);
        }
    }
}
