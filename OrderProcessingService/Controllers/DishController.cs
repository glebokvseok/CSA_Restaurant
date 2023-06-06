using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingService.Interfaces;
using OrderProcessingService.Models;

namespace OrderProcessingService.Controllers; 

[ApiController]
[Route("dish")]
public class DishController : Controller {
    public DishController(IDishAccessLayer dishAccessLayer) {
        _dishAccessLayer = dishAccessLayer;
    }
    
    [Authorize]
    [HttpGet("get")]
    public async Task<IActionResult> GetDish([FromQuery] int id) {
        try {
            var dish = await _dishAccessLayer.GetDish(id);
            if (dish is null) {
                return NotFound($"Dish with id {id} does not exist");
            }

            return Ok(JsonSerializer.Serialize(dish));
        } catch {
            return BadRequest(ErrorResponseMessage);
        }
    }

    [Authorize(Roles = "Manager")]
    [HttpPost("add")]
    public async Task<IActionResult> AddDish([FromBody] Dish dish) {
        try {
            var  id = await _dishAccessLayer.AddDish(dish);
            
            return Ok($"Dish with id {id} was successfully added");
        } catch {
            return BadRequest(ErrorResponseMessage);
        }
    }

    [Authorize(Roles = "Manager")]
    [HttpPatch("update")]
    public async Task<IActionResult> UpdateDish([FromBody] Dish dish) {
        try {
            if (!await _dishAccessLayer.CheckDishExistence(dish.Id)) {
                return NotFound($"Dish with id {dish.Id} does not exist");
            }

            await _dishAccessLayer.UpdateDish(dish);
            
            return Ok($"Dish with id {dish.Id} was successfully updated");
        } catch {
            return BadRequest(ErrorResponseMessage);
        }
    }

    [Authorize(Roles = "Manager")]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteDish([FromBody] int id) {
        try {
            if (!await _dishAccessLayer.CheckDishExistence(id)) {
                return NotFound($"Dish with id {id} does not exist");
            }

            await _dishAccessLayer.DeleteDish(id);
            
            return Ok($"Dish with id {id} was successfully deleted");
        } catch {
            return BadRequest(ErrorResponseMessage);
        }
    }

    [Authorize]
    [HttpGet("menu")]
    public async Task<IActionResult> GetMenu() {
        try {
            return Ok(JsonSerializer.Serialize(await _dishAccessLayer.GetMenu()));
        } catch {
            return BadRequest(ErrorResponseMessage);
        }
    }

    private const string ErrorResponseMessage = "Service is temporarily unavailable";

    private readonly IDishAccessLayer _dishAccessLayer;
}