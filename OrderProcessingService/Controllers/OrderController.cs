using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingService.Interfaces;
using OrderProcessingService.Models;

namespace OrderProcessingService.Controllers; 

[ApiController]
[Route("order")]
public class OrderController : Controller {
    public OrderController(
        ICookerService cookerService,
        IUserAccessLayer userAccessLayer,
        IDishAccessLayer dishAccessLayer,
        IOrderAccessLayer orderAccessLayer,
        ISemaphoreWrapper semaphoreWrapper) {
        _cookerService = cookerService;
        _userAccessLayer = userAccessLayer;
        _dishAccessLayer = dishAccessLayer;
        _orderAccessLayer = orderAccessLayer;
        _semaphoreWrapper = semaphoreWrapper;
    }

    [Authorize]
    [HttpPost("place")]
    public async Task<IActionResult> PlaceOrder([FromBody] Order order) {
        int id;
        await _semaphoreWrapper.WaitAsync();
        try {
            if (!await _userAccessLayer.CheckUserExistence(order.UserId)) {
                return NotFound($"User with id {order.UserId} does not exist");
            }
            
            var dishes = new List<Dish>();
            order.Dishes = CompressItems(order.Dishes);
            foreach (var item in order.Dishes) {
                var dish = await _dishAccessLayer.GetDish(item.DishId);
                if (dish is null) {
                    return NotFound($"Dish with id {item.DishId} does not exist");
                }

                if (!dish.IsAvailable) {
                    return BadRequest($"Dish with id {item.DishId} is not available now");
                }

                if (dish.Quantity < item.Quantity) {
                    return BadRequest($"Dish with id {item.DishId} is only available in quantity {dish.Quantity} or lower");
                }

                item.Price = dish.Price;
                dish.Quantity -= item.Quantity;
                dishes.Add(dish);
            }

            id = await _orderAccessLayer.AddOrder(order);
            await _dishAccessLayer.UpdateDishes(dishes);
        } catch {
            return BadRequest(ErrorResponseMessage);
        } finally {
            _semaphoreWrapper.Release();
        }
        
        _cookerService.StartCooking(id);
        
        return Ok($"Order with id {id} has been successfully placed");
    }

    [Authorize]
    [HttpGet("get")]
    public async Task<IActionResult> GetOrder([FromQuery] int id) {
        try {
            var order = await _orderAccessLayer.GetOrder(id);
            if (order is null) {
                return NotFound("Order with this id does not exist");
            }

            return Ok(JsonSerializer.Serialize(order));
        } catch {
            return BadRequest(ErrorResponseMessage);
        }
    }

    private const string ErrorResponseMessage = "Service is temporarily unavailable";

    private static IEnumerable<DishItem> CompressItems(IEnumerable<DishItem> items) {
        var dishQuantity = new Dictionary<int, int>();
        foreach (var item in items) {
            if (dishQuantity.ContainsKey(item.DishId)) {
                dishQuantity[item.DishId] += item.Quantity;
            } else {
                dishQuantity.Add(item.DishId, item.Quantity);
            }
        }

        var compressedItems = new List<DishItem>();
        foreach (var (dishId, quantity) in dishQuantity) {
            compressedItems.Add(new DishItem {DishId = dishId, Quantity = quantity});
        }

        return compressedItems;
    }

    private readonly ICookerService _cookerService;
    private readonly IUserAccessLayer _userAccessLayer;
    private readonly IDishAccessLayer _dishAccessLayer;
    private readonly IOrderAccessLayer _orderAccessLayer;
    private readonly ISemaphoreWrapper _semaphoreWrapper;
}
