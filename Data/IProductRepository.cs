namespace LegacyOrderService.Data;

public interface IProductRepository
{
    decimal GetPrice(string productName);
}
