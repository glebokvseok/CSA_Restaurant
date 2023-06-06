using System.Text.Json.Serialization;

namespace OrderProcessingService.Models;

public record MenuItem {
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = null!;
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("price")] public decimal Price { get; set; }
    [JsonPropertyName("quantity")] public int Quantity { get; set; }
}