using System.Collections.Generic;
using System.Threading.Tasks;
using OrderProcessingService.Models;

namespace OrderProcessingService.Interfaces;

public interface IDishAccessLayer {
    public Task<bool> CheckDishExistence(int id);
    public Task<Dish?> GetDish(int id);
    
    public Task<int> AddDish(Dish dish);
    public Task UpdateDish(Dish dish);
    public Task DeleteDish(int id);
    public Task<IEnumerable<MenuItem>> GetMenu();
    public Task UpdateDishes(IEnumerable<Dish> dishes);
}