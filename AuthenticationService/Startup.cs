using AuthenticationService.Extensions;
using AuthenticationService.Interfaces;
using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vault;

namespace AuthenticationService; 

public class Startup {
    public Startup(IConfiguration configuration) => _configuration = configuration;
    
    public void ConfigureServices(IServiceCollection services) {
        var host = _configuration["RestaurantDataBase:Host"];
        var port = _configuration["RestaurantDataBase:Port"];
        var name = _configuration["RestaurantDataBase:Name"];
        var user = _configuration["RestaurantDataBase:User"];
        var password = _configuration["RestaurantDataBase:Password"];
        var usersTableName = _configuration["RestaurantDataBase:UsersTableName"];
        var sessionsTableName = _configuration["RestaurantDataBase:SessionsTableName"];
        
        HostExtension.SetConnectionString(host, port, name, user, password);

        services.AddControllers();
        
        services.AddSingleton<ITokenCreator, TokenCreator>();
        services.AddSingleton<ICryptographer, Cryptographer>();
        services.AddSingleton<ITokenDataManager, TokenDataManager>();
        services.AddSingleton<IUserAccessLayer>(
            new UserAccessLayer(host, port, name, user, password, usersTableName, sessionsTableName));
        
        services.AddSingleton<ISessionAccessLayer>(new SessionAccessLayer(host, port, name, user, password, sessionsTableName));

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