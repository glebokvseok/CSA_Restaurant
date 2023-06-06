using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderProcessingService.Models;

public record Dish {
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")][Required] public string Name { get; set; } = null!;
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("price")][Required] public decimal Price { get; set; }
    [JsonPropertyName("quantity")][Required] public int Quantity { get; set; }
    [JsonPropertyName("is_available")] public bool IsAvailable { get; set; }
    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
}