using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AuthenticationService.Models;
using AuthenticationService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vault;

namespace AuthenticationService.Controllers; 

[ApiController]
[Route("user")]
public class UserController : Controller {
    public UserController(
        ITokenCreator tokenCreator, 
        ICryptographer cryptographer,
        IUserAccessLayer userDatabase,
        ITokenDataManager tokenDataManager,
        ISessionAccessLayer sessionDatabase) {
        _tokenCreator = tokenCreator;
        _cryptographer = cryptographer;
        _userDatabase = userDatabase;
        _tokenDataManager = tokenDataManager;
        _sessionDatabase = sessionDatabase;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel) {
        int id;
        
        if ((int) registrationModel.Role < 0 || (int) registrationModel.Role > RoleMaxValue) {
            return BadRequest("Unknown role");
        }
        
        try {
            if (await _userDatabase.CheckUsernameExistence(registrationModel.Username)) {
                return BadRequest("Account with this username already exists");
            }

            if (await _userDatabase.CheckEmailExistence(registrationModel.Login)) {
                return BadRequest("Account with this login(email) already exists");
            }
            
            registrationModel.Password = _cryptographer.Encrypt(registrationModel.Password);
            
            id = await _userDatabase.AddUser(registrationModel);
        } catch {
            return BadRequest(ErrorResponseMessage);
        }

        var token = _tokenCreator.CreateToken(GetClaims(registrationModel.Login, registrationModel.Role), TokenLifetime);

        var session = new Session {
            UserId = id,
            Token = token,
            ExpiresAt = _tokenDataManager.GetExpirationTime(token, new TokenData().ValidationParameters),
        };

        try {
            await _sessionDatabase.AddSession(session);
        } catch {
            return BadRequest(ErrorResponseMessage);
        }

        return Ok(token);
    }

    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize([FromBody] AuthorizationModel authorizationModel) {
        string? password;
        
        try {
            password = await _userDatabase.GetUserPassword(authorizationModel.Login);
        } catch {
            return BadRequest("Service is temporarily unavailable");
        }

        if (password is null) {
            return NotFound("User with this login does not exist");
        }

        if (password != _cryptographer.Encrypt(authorizationModel.Password)) {
            return BadRequest("Incorrect password");
        }

        try {
            var data = await _userDatabase.GetUserRoleAndId(authorizationModel.Login);
            var token = _tokenCreator.CreateToken(GetClaims(authorizationModel.Login, data.Item1), TokenLifetime);
            var session = new Session {
                UserId = data.Item2,
                Token = token,
                ExpiresAt = _tokenDataManager.GetExpirationTime(token, new TokenData().ValidationParameters),
            };

            await _sessionDatabase.UpdateSession(session);

            return Ok(token);
        } catch {
           return BadRequest(ErrorResponseMessage);
        }
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<IActionResult> GetUser() {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        try {
            return Ok(JsonSerializer.Serialize(await _userDatabase.GetUser(token)));
        } catch {
            return BadRequest(ErrorResponseMessage);
        }
    }

    private const int RoleMaxValue = 4;
    private const int TokenLifetime = 15;
    private const string ErrorResponseMessage = "Service is temporarily unavailable";

    private static IEnumerable<Claim> GetClaims(string login, Role role) => new List<Claim> {
        new (ClaimsIdentity.DefaultNameClaimType, login),
        new (ClaimsIdentity.DefaultRoleClaimType, role.ToString())
    };

    private readonly ITokenCreator _tokenCreator;
    private readonly ICryptographer _cryptographer;
    private readonly IUserAccessLayer _userDatabase;
    private readonly ITokenDataManager _tokenDataManager;
    private readonly ISessionAccessLayer _sessionDatabase;
}