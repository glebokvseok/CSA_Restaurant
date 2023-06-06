using System;
using System.Text.Json.Serialization;

namespace AuthenticationService.Models;

public record User {
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; } = null!;
    [JsonPropertyName("username")] public string Username { get; set; } = null!;
    [JsonIgnore] public Role Role { get; set; }
    [JsonPropertyName("role")] public string? RoleName { get; set; }
    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("expires_at")] public DateTime ExpiresAt { get; set; }
}