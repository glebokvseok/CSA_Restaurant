using System.Threading.Tasks;
using AuthenticationService.Models;

namespace AuthenticationService.Interfaces; 

public interface ISessionAccessLayer {
    public Task AddSession(Session session);
    public Task UpdateSession(Session session);
}