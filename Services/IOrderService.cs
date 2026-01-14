using LegacyOrderService.Models;

namespace LegacyOrderService.Services
{
    public interface IOrderService
    {
        Order CreateOrder(string customerName, string productName, int quantity, decimal price);
        decimal CalculateTotal(Order order);
        Task SaveOrderAsync(Order order);
    }
}
