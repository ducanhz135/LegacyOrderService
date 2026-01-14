using LegacyOrderService.Models;

namespace LegacyOrderService.Data;

public interface IOrderRepository
{
    Task InitializeDatabaseAsync();
    Task SaveAsync(Order order);
}
