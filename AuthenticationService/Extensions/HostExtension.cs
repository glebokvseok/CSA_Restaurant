using System;
using System.IO;
using Dapper;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace AuthenticationService.Extensions; 

public static class HostExtension {
    public static IHost MigrationUp(this IHost host) {
        try {
            var request = File.ReadAllText(MigrationSchemaPath);
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Execute(request);
        } catch {
            Console.WriteLine("Migration up failed, tables was not created");
        }
        
        return host;
    }
    
    public static void SetConnectionString(string host, string port, string database, string user, string password) =>
        _connectionString = $"Host={host};Port={port};Database={database};User ID={user};Password={password}";

    private const string MigrationSchemaPath = "Schemas/Migration.sql";

    private static string _connectionString = null!;
}