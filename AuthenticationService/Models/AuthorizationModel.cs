using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Models;

public record AuthorizationModel {
    [Required] [EmailAddress] public string Login { get; set; } = null!;
    [Required] [RegularExpression(Config.PasswordValidationPattern)] public string Password { get; set; } = null!;
}