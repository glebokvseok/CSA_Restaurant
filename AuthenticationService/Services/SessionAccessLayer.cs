using System.Threading.Tasks;
using AuthenticationService.Interfaces;
using AuthenticationService.Models;
using Dapper;
using Npgsql;

namespace AuthenticationService.Services; 

public class SessionAccessLayer : DataAccessLayer, ISessionAccessLayer {
    public SessionAccessLayer(string host, string port, string database, string user, string password, string table)
        : base(host, port, database, user, password) =>
            _sessionsTableName = table;
    
    public async Task AddSession(Session session) {
        var request = $"INSERT INTO {_sessionsTableName} (user_id, session_token, expires_at) " +
                            "VALUES (@userId, @token, @expiresAt);";
        
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(request, session);
    }

    public async Task UpdateSession(Session session) {
        var request = $"UPDATE {_sessionsTableName} SET (expires_at, session_token) = (@expiresAt, @token) " +
                            "WHERE user_id = @userId;";
        
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(request, session);
    }
    
    private readonly string _sessionsTableName;
}