using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationService.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Models; 

public class TokenDataManager : ITokenDataManager {
    public IEnumerable<Claim> GetClaims(string token, TokenValidationParameters validationParameters) {
        return new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _).Claims;
    }

    public DateTime GetExpirationTime(string token, TokenValidationParameters validationParameters) {
        new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var validatedToken);
        return validatedToken.ValidTo.ToUniversalTime();
    }
}