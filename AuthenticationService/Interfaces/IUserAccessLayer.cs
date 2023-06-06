using System.Threading.Tasks;
using AuthenticationService.Models;

namespace AuthenticationService.Interfaces; 

public interface IUserAccessLayer {
    public Task<int> AddUser(RegistrationModel registrationModel);
    public Task<bool> CheckUsernameExistence(string username);
    public Task<bool> CheckEmailExistence(string email);
    public Task<string?> GetUserPassword(string login);
    public Task<User> GetUser(string token);
    public Task<(Role, int)> GetUserRoleAndId(string login);
}