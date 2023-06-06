using System;
using System.Threading.Tasks;
using OrderProcessingService.Interfaces;
using OrderProcessingService.Models;

namespace OrderProcessingService.Services; 

public class CookerService : ICookerService {
    public CookerService(IOrderAccessLayer orderAccessLayer) {
        _orderAccessLayer = orderAccessLayer;
    }
    public async void StartCooking(int orderId) {
        var random = new Random();
        var stages = new[] {Status.InProcess, Status.Ready};
        foreach (var stage in stages) {
            await Wait(random);
            await _orderAccessLayer.UpdateOrder(orderId, stage);
        }
    }

    private const int MinSecondsDelay = 30;
    private const int MaxSecondsDelay = 90;

    private static async Task Wait(Random random) 
        => await Task.Delay(TimeSpan.FromSeconds(random.Next(MinSecondsDelay, MaxSecondsDelay)));

    private readonly IOrderAccessLayer _orderAccessLayer;
}