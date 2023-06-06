namespace AuthenticationService; 

public static class Config {
    public const string UsernameValidationPattern = @"^(?!.*[^A-Za-z0-9]).{4,12}";
    public const string PasswordValidationPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?!.*[^A-Za-z0-9]).{8,16}$";
}