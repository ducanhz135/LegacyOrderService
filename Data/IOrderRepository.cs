using LegacyOrderService.Models;

namespace LegacyOrderService.Data
{
    public interface IOrderRepository
    {
        void InitializeDatabase();
        void Save(Order order);
    }
}
