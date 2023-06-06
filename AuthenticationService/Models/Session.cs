using System;

namespace AuthenticationService.Models; 

public class Session {
    public int UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Token { get; set; } = null!;
}