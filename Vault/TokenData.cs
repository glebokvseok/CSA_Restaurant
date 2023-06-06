using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Vault; 

public class TokenData {
    public const string Issuer = "AuthenticationServer";
    public const string Audience = "RestaurantStaff";
    public const string Signature = "#SecretS1gnature218";

    public TokenData() {
        ValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true, 
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Signature)),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
        };
    }
    
    public TokenValidationParameters ValidationParameters { get; }
}