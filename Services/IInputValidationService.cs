namespace LegacyOrderService.Services
{
    public interface IInputValidationService
    {
        bool ValidateCustomerName(string name, out string errorMessage);
        bool ValidateProductName(string productName, out string errorMessage);
        bool ValidateQuantity(string quantityInput, out int quantity, out string errorMessage);
    }
}
