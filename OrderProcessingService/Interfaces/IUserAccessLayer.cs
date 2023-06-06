using System.Threading.Tasks;

namespace OrderProcessingService.Interfaces; 

public interface IUserAccessLayer {
    public Task<bool> CheckUserExistence(int id);
}