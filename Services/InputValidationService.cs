namespace LegacyOrderService.Services
{
    public class InputValidationService : IInputValidationService
    {
        public bool ValidateCustomerName(string name, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = "Customer name cannot be empty.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public bool ValidateProductName(string productName, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                errorMessage = "Product name cannot be empty.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public bool ValidateQuantity(string quantityInput, out int quantity, out string errorMessage)
        {
            if (!int.TryParse(quantityInput, out quantity) || quantity <= 0)
            {
                errorMessage = "Quantity must be a positive number.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}
