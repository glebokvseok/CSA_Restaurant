using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Models;

public record RegistrationModel {
    [Required] public Role Role { get; set; }
    [Required] [EmailAddress] public string Login { get; set; } = null!;
    [Required] [RegularExpression(Config.UsernameValidationPattern)] public string Username { get; set; } = null!;
    [Required] [RegularExpression(Config.PasswordValidationPattern)] public string Password { get; set; } = null!;
}