using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OrderProcessingService.Extensions;

namespace OrderProcessingService;

public static class Program {
    private static void Main(string[] args) => CreateHostBuilder(args).Build().MigrationUp().Run();

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
}