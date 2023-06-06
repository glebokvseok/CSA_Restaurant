using System.Collections.Generic;
using System.Threading.Tasks;
using OrderProcessingService.Models; 

namespace OrderProcessingService.Interfaces; 

public interface IOrderAccessLayer {
    public Task<int> AddOrder(Order order);
    public Task UpdateOrder(int id, Status status);
    public Task<Order?> GetOrder(int id);
}