using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderProcessingService.Models;

public record Order {
    [Required][JsonPropertyName("user_id")] public int UserId { get; set; }
    [Required][JsonPropertyName("dishes")] public IEnumerable<DishItem> Dishes { get; set; } = null!;
    [JsonIgnore] public Status Status { get; set; }
    [JsonPropertyName("status")] public string? StatusName { get; set; }
    [JsonPropertyName("special_requests")] public string? SpecialRequests { get; set; }
}
