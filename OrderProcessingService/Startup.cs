using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessingService.Extensions;
using OrderProcessingService.Interfaces;
using OrderProcessingService.Services;
using Vault;

namespace OrderProcessingService; 

public class Startup {
    public Startup(IConfiguration configuration) => _configuration = configuration;
    
    public void ConfigureServices(IServiceCollection services) {
        var host = _configuration["RestaurantDataBase:Host"];
        var port = _configuration["RestaurantDataBase:Port"];
        var name = _configuration["RestaurantDataBase:Name"];
        var user = _configuration["RestaurantDataBase:User"];
        var password = _configuration["RestaurantDataBase:Password"];
        var usersTableName = _configuration["RestaurantDataBase:UsersTableName"];
        var dishesTableName = _configuration["RestaurantDataBase:DishesTableName"];
        var ordersTableName = _configuration["RestaurantDataBase:OrdersTableName"];
        var orderDishTableName = _configuration["RestaurantDataBase:OrderDishTableName"];
        
        HostExtension.SetConnectionString(host, port, name, user, password);
        
        var orderAccessLayer = new OrderAccessLayer(host, port, name, user, password, ordersTableName, orderDishTableName);
        
        services.AddControllers();
        
        services.AddSingleton<ICookerService, CookerService>();
        services.AddSingleton<ISemaphoreWrapper, SemaphoreWrapper>();
        services.AddSingleton<IOrderAccessLayer>(orderAccessLayer);
        services.AddSingleton<ICookerService>(new CookerService(orderAccessLayer));
        services.AddSingleton<IUserAccessLayer>(new UserAccessLayer(host, port, name, user, password, usersTableName));
        services.AddSingleton<IDishAccessLayer>(new DishAccessLayer(host, port, name, user, password, dishesTableName));
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenData().ValidationParameters;
        });
    }

    public void Configure(IApplicationBuilder application, IWebHostEnvironment environment) {
        if (environment.IsDevelopment()) {
            application.UseDeveloperExceptionPage();
        }
        
        application.UseCors(builder => {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });

        application.UseDefaultFiles();
        application.UseStaticFiles();
        application.UseRouting();
        application.UseAuthentication();
        application.UseAuthorization();
        
        application.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
    
    private readonly IConfiguration _configuration;
}