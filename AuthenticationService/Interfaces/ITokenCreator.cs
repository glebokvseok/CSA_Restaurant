using System.Collections.Generic;
using System.Security.Claims;

namespace AuthenticationService.Interfaces; 

public interface ITokenCreator {
    public string CreateToken(IEnumerable<Claim> claims, int lifetime);
}