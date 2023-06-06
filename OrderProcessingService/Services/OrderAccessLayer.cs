using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using OrderProcessingService.Interfaces;
using OrderProcessingService.Models;

namespace OrderProcessingService.Services; 

public class OrderAccessLayer : DataAccessLayer, IOrderAccessLayer {
    public OrderAccessLayer
        (string host, string port, string database, string user, string password, string ordersTableName, string orderDishTableName)
        : base(host, port, database, user, password) {
        _ordersTableName = ordersTableName;
        _orderDishTableName = orderDishTableName;
    }
    
    public async Task<int> AddOrder(Order order) {
        var request = 
            $"INSERT INTO {_ordersTableName} (user_id, status, special_requests, created_at, updated_at) " +
            "VALUES (@userId, @status, @specialRequests, @createdAt, @updatedAt) " +
            "RETURNING id;";
        
        var timestamp = DateTime.Now.ToUniversalTime();
        var parameters = new {
            UserId = order.UserId,
            Status = Status.Waiting,
            SpecialRequests = order.SpecialRequests,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
        };
        
        await using var connection = new NpgsqlConnection(ConnectionString);
        var id =  (await connection.QueryAsync<int>(request, parameters)).First();
        await AddOrderDishes(id, order.Dishes);
        return id;
    }

    public async Task UpdateOrder(int id, Status status) {
        var request = $"UPDATE {_ordersTableName} SET (status, updated_at) = (@status, @updatedAt) WHERE id = @id;";
        var parameters = new {
            Id = id,
            Status = status,
            UpdatedAt = DateTime.Now.ToUniversalTime(),
        };

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(request, parameters);
    }

    public async Task<Order?> GetOrder(int id) {
        var request = $"SELECT user_id AS userId, status, special_requests AS specialRequests FROM {_ordersTableName} WHERE id = @id;";
        await using var connection = new NpgsqlConnection(ConnectionString);
        var order = (await connection.QueryAsync<Order?>(request, new {id})).FirstOrDefault();
        if (order is null) {
            return order;
        }

        order.StatusName = order.Status.ToString();
        order.Dishes = await GetDishes(id);
        return order;
    }

    private readonly string _ordersTableName;
    private readonly string _orderDishTableName;
    
    private async Task<IEnumerable<DishItem>> GetDishes(int id) {
        var request = $"SELECT dish_id AS dishId, quantity, price FROM {_orderDishTableName} WHERE order_id = @id;";
        var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<DishItem>(request, new {id}) ?? new List<DishItem>();
    }

    private async Task AddOrderDishes(int orderId, IEnumerable<DishItem> items) {
        var request =
            $"INSERT INTO {_orderDishTableName} (order_id, dish_id, quantity, price) " +
            "VALUES (@orderId, @dishId, @quantity, @price)";

        await using var connection = new NpgsqlConnection(ConnectionString);
        foreach (var item in items) {
            var parameters = new {
                OrderId = orderId,
                DishId = item.DishId,
                Quantity = item.Quantity,
                Price = item.Price,
            };

            await connection.ExecuteAsync(request, parameters);
        }
    }
}