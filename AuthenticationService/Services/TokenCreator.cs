using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Vault;

namespace AuthenticationService.Services; 

public class TokenCreator : ITokenCreator {
    public string CreateToken(IEnumerable<Claim> claims, int lifetime) {
        var jwt = new JwtSecurityToken(
            issuer: TokenData.Issuer,
            audience: TokenData.Audience,
            claims: claims,
            notBefore: DateTime.Now.ToUniversalTime(),
            expires: DateTime.Now.AddMinutes(lifetime).ToUniversalTime(),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenData.Signature)),
                SecurityAlgorithms.HmacSha256
            )    
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}