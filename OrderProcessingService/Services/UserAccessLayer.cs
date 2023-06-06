using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using OrderProcessingService.Interfaces;

namespace OrderProcessingService.Services; 

public class UserAccessLayer : DataAccessLayer, IUserAccessLayer {
    public UserAccessLayer(string host, string port, string database, string user, string password, string table)
        : base(host, port, database, user, password) =>
            _usersTableName = table;

    public async Task<bool> CheckUserExistence(int id) {
        var request = $"SELECT COUNT(1) FROM {_usersTableName} WHERE id = @id;";
        await using var connection = new NpgsqlConnection(ConnectionString);
        return (await connection.QueryAsync<int>(request, new {id})).FirstOrDefault() == 1;
    }

    private readonly string _usersTableName;
}