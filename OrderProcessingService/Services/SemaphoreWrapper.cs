using System.Threading;
using System.Threading.Tasks;
using OrderProcessingService.Interfaces;

namespace OrderProcessingService.Services; 

public class SemaphoreWrapper : ISemaphoreWrapper {
    public SemaphoreWrapper() {
        _semaphore = new SemaphoreSlim(1);
    }
    
    public async Task WaitAsync() {
        await _semaphore.WaitAsync();
    }

    public void Release() {
        _semaphore.Release();
    }

    private readonly SemaphoreSlim _semaphore;
}