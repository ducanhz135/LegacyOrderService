using Microsoft.Data.Sqlite;
using LegacyOrderService.Models;
using Microsoft.Extensions.Configuration;

namespace LegacyOrderService.Data;

public class OrderRepository : IOrderRepository
{
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OrderDatabase")
                ?? throw new InvalidOperationException("OrderDatabase connection string not found in configuration");
        }

        public async Task InitializeDatabaseAsync()
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id TEXT PRIMARY KEY,
                    CustomerName TEXT NOT NULL,
                    ProductName TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Price REAL NOT NULL,
                    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
                );
                -- create indexes for performance improvement in the future
                CREATE INDEX IF NOT EXISTS IX_Orders_CreatedAt ON Orders(CreatedAt);
                CREATE INDEX IF NOT EXISTS IX_Orders_CustomerName ON Orders(CustomerName);
                CREATE INDEX IF NOT EXISTS IX_Orders_ProductName ON Orders(ProductName);";

            await command.ExecuteNonQueryAsync();
        }

        public async Task SaveAsync(Order order)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Orders (Id, CustomerName, ProductName, Quantity, Price)
                VALUES (@Id, @CustomerName, @ProductName, @Quantity, @Price)";

            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@CustomerName", order.CustomerName);
            command.Parameters.AddWithValue("@ProductName", order.ProductName);
            command.Parameters.AddWithValue("@Quantity", order.Quantity);
            command.Parameters.AddWithValue("@Price", order.Price);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SeedBadDataAsync()
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            await using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Orders (Id, CustomerName, ProductName, Quantity, Price) VALUES (@Id, @CustomerName, @ProductName, @Quantity, @Price)";
            
            command.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
            command.Parameters.AddWithValue("@CustomerName", "John");
            command.Parameters.AddWithValue("@ProductName", "Widget");
            command.Parameters.AddWithValue("@Quantity", 9999);
            command.Parameters.AddWithValue("@Price", 9.99);
            
            await command.ExecuteNonQueryAsync();
        }
    }
