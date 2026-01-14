using System;
using Microsoft.Data.Sqlite;
using LegacyOrderService.Models;

namespace LegacyOrderService.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, "orders.db")}";

        public async Task InitializeDatabaseAsync()
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Orders (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerName TEXT NOT NULL,
                    ProductName TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Price REAL NOT NULL,
                    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP
                );";

            await command.ExecuteNonQueryAsync();
        }

        public async Task SaveAsync(Order order)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Orders (CustomerName, ProductName, Quantity, Price)
                VALUES (@CustomerName, @ProductName, @Quantity, @Price)";

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
            command.CommandText = "INSERT INTO Orders (CustomerName, ProductName, Quantity, Price) VALUES (@CustomerName, @ProductName, @Quantity, @Price)";
            
            command.Parameters.AddWithValue("@CustomerName", "John");
            command.Parameters.AddWithValue("@ProductName", "Widget");
            command.Parameters.AddWithValue("@Quantity", 9999);
            command.Parameters.AddWithValue("@Price", 9.99);
            
            await command.ExecuteNonQueryAsync();
        }
    }
}
