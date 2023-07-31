using System.Data.Common;
using Dapper;
using MySql.Data.MySqlClient;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public interface IMySqlContext
{
    Task<List<UserViewModel>> GetUsers();
    Task<User> CreateUser(User user);
}

public class MySqlContext : IMySqlContext
{
    private readonly DbConnection _db;

    public MySqlContext(IConfiguration config)
    {
        _db = new MySqlConnection(config.GetConnectionString("mysql"));
    }

    public async Task<List<UserViewModel>> GetUsers()
    {
        return (await _db.QueryAsync<UserViewModel>($@"select 
                    username {nameof(UserViewModel.Username)},
                    email {nameof(UserViewModel.Email)},
                    firstname {nameof(UserViewModel.FirstName)},
                    lastname {nameof(UserViewModel.LastName)}
                    from Users
                    order by username")).ToList();
    }

    public async Task<User> CreateUser(User user)
    {
        await _db.ExecuteAsync(
            @"insert into Users (id,username,email,firstname,lastname,salt) values(@id,@username,@email,@firstname,@lastname,@salt)",
            new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                firstname = user.FirstName,
                lastname = user.LastName,
                salt = user.Salt
            });

        return (await _db.QueryAsync<User>($@"select 
                    username {nameof(User.Username)},
                    email {nameof(User.Email)},
                    firstname {nameof(User.FirstName)},
                    lastname {nameof(User.LastName)},
                    salt {nameof(User.Salt)}
                    from Users where id = @id", new { id = user.Id }))
            .First();
    }

}