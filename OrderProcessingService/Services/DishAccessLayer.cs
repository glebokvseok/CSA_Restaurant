using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using OrderProcessingService.Interfaces;
using OrderProcessingService.Models;

namespace OrderProcessingService.Services; 

public class DishAccessLayer : DataAccessLayer, IDishAccessLayer {
    public DishAccessLayer(string host, string port, string database, string user, string password, string table)
        : base(host, port, database, user, password) =>
            _dishesTableName = table;

    public async Task<bool> CheckDishExistence(int id) {
        var request = $"SELECT COUNT(1) FROM {_dishesTableName} WHERE id = @id;";
        await using var connection = new NpgsqlConnection(ConnectionString);
        return (await connection.QueryAsync<int>(request, new {id})).FirstOrDefault() == 1;
    }
    
    public async Task<Dish?> GetDish(int id) {
        var request = 
            "SELECT id, name, description, price, quantity, " +
            "is_available AS isAvailable, created_at AS createdAt, updated_at AS updatedAt " +
            $"FROM {_dishesTableName} WHERE id = @id;";
        
        var connection = new NpgsqlConnection(ConnectionString);
        return (await connection.QueryAsync<Dish?>(request, new {id})).FirstOrDefault();
    }

    public async Task<int> AddDish(Dish dish) {
        var request = 
            $"INSERT INTO {_dishesTableName} (name, description, price, quantity, is_available, created_at, updated_at) " +
            "VALUES (@name, @description, @price, @quantity, @isAvailable, @createdAt, @updatedAt)" +
            "RETURNING id;";
        
        dish.IsAvailable = dish.Quantity > 0;
        dish.Quantity = Math.Max(dish.Quantity, 0);
        dish.CreatedAt = dish.UpdatedAt = DateTime.Now.ToUniversalTime();

        await using var connection = new NpgsqlConnection(ConnectionString);
        return (await connection.QueryAsync<int>(request,dish)).First();
    }

    public async Task UpdateDish(Dish dish) {
        var request = 
            $"UPDATE {_dishesTableName} " +
            "SET (name, description, price, quantity, is_available, updated_at) = " +
            "(@name, @description, @price, @quantity, @isAvailable, @updatedAt) " +
            "WHERE id = @id;";
        
        var connection = new NpgsqlConnection(ConnectionString);
        dish.IsAvailable = dish.Quantity > 0;
        dish.Quantity = Math.Max(dish.Quantity, 0);
        dish.UpdatedAt = DateTime.Now.ToUniversalTime();
        await connection.ExecuteAsync(request, dish);
    }

    public async Task DeleteDish(int id) {
        var request = $"DELETE FROM {_dishesTableName} WHERE id = @id;";
        var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(request, new {id});
    }

    public async Task<IEnumerable<MenuItem>> GetMenu() {
        var request = $"SELECT id, name, description, price, quantity FROM {_dishesTableName} WHERE is_available;";
        var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<MenuItem>(request) ?? new List<MenuItem>();
    }

    public async Task UpdateDishes(IEnumerable<Dish> dishes) {
        foreach (var dish in dishes) {
            await UpdateDish(dish);
        }
    }

    private readonly string _dishesTableName;
}