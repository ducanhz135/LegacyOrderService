using LegacyOrderService.Models;

namespace LegacyOrderService.Services
{
    public interface IOrderService
    {
        Order CreateOrder(string customerName, string productName, int quantity, double price);
        double CalculateTotal(Order order);
        void SaveOrder(Order order);
    }
}
