using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Interfaces; 

public interface ITokenDataManager {
    public IEnumerable<Claim> GetClaims(string token, TokenValidationParameters validationParameters);
    public DateTime GetExpirationTime(string token, TokenValidationParameters validationParameters);
}