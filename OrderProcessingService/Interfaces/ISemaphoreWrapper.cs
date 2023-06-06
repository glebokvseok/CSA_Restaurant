using System.Threading.Tasks;

namespace OrderProcessingService.Interfaces; 

public interface ISemaphoreWrapper {
    public Task WaitAsync();
    public void Release();
}