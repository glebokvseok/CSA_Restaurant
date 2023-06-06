using System;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationService.Interfaces;
using AuthenticationService.Models;
using Dapper;
using Npgsql;

namespace AuthenticationService.Services; 

public class UserAccessLayer : DataAccessLayer, IUserAccessLayer {
    public UserAccessLayer
        (string host, string port, string database, string user, string password, string usersTableName, string sessionsTableName)
        : base(host, port, database, user, password) {
        _usersTableName = usersTableName;
        _sessionsTableName = sessionsTableName;
    }

    public async Task<int> AddUser(RegistrationModel registrationModel) {
        var request = 
            $"INSERT INTO {_usersTableName} (username, email, password_hash, role, created_at, updated_at) " +
            "VALUES (@username, @email, @password, @role, @createdAt, @updatedAt) " +
            "RETURNING id";

        var timestamp = DateTime.Now.ToUniversalTime();
        var user = new {
            Username = registrationModel.Username,
            Email = registrationModel.Login,
            Password = registrationModel.Password,
            Role = registrationModel.Role,
            CreatedAt = timestamp,
            UpdatedAt = timestamp,
        };

        await using var connection = new NpgsqlConnection(ConnectionString);
        return (await connection.QueryAsync<int>(request, user)).First();
    }

    public async Task<bool> CheckUsernameExistence(string username) {
        var request = $"SELECT COUNT(1) FROM {_usersTableName} WHERE username = @username;";
        await using var connection = new NpgsqlConnection(ConnectionString);
        return (await connection.QueryAsync<int>(request, new {username})).FirstOrDefault() == 1;
    }

    public async Task<bool> CheckEmailExistence(string email) {
        var request = $"SELECT COUNT(1) FROM {_usersTableName} WHERE email = @email;";
        await using var connection = new NpgsqlConnection(ConnectionString);
        return (await connection.QueryAsync<int>(request, new {email})).FirstOrDefault() == 1;
    }

    public async Task<string?> GetUserPassword(string login) {
        var request = $"SELECT password_hash FROM {_usersTableName} WHERE email = @login";
        await using var connection = new NpgsqlConnection(ConnectionString);
        return (await connection.QueryAsync<string?>(request, new {login})).FirstOrDefault();
    }

    public async Task<User> GetUser(string token) {
        var request = 
            "SELECT users.id as id, username, email, role, created_at AS createdAt, updated_at AS updatedAt, expires_at AS expiresAt " +
            $"FROM {_usersTableName} LEFT JOIN {_sessionsTableName} ON users.id = user_id " +
            "WHERE session_token = @token;";
        
        await using var connection = new NpgsqlConnection(ConnectionString);
        var user = (await connection.QueryAsync<User>(request, new {token})).First();
        user.RoleName = user.Role.ToString();
        return user;
    }
    
    public async Task<(Role, int)> GetUserRoleAndId(string login) {
        var request = $"SELECT role, id FROM {_usersTableName} WHERE email = @login";
        await using var connection = new NpgsqlConnection(ConnectionString);
        return (await connection.QueryAsync<(Role, int)>(request, new {login})).First();
    }

    private readonly string _usersTableName;
    private readonly string _sessionsTableName;
}