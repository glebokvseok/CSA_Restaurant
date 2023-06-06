using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderProcessingService.Models; 

public record DishItem {
    [Required][JsonPropertyName("dish_id")] public int DishId { get; set; }
    [Required][JsonPropertyName("quantity")] public int Quantity { get; set; }
    [JsonPropertyName("price")] public decimal Price { get; set; }
}