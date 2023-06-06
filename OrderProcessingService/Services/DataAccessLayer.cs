namespace OrderProcessingService.Services;

public abstract class DataAccessLayer {
    protected DataAccessLayer(string host, string port, string database, string user, string password) {
        ConnectionString = $"Host={host};Port={port};Database={database};User ID={user};Password={password}";
    }
    
    protected readonly string ConnectionString;
}