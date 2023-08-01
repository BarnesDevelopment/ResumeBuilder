using System.Data.Common;
using Dapper;
using MySql.Data.MySqlClient;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public interface IMySqlContext
{
    Task<List<UserViewModel>> GetUsers();
    Task<User> CreateUser(User user);
    Task<bool> UpdateSalt(User user);
    Task<bool> DeleteUser(Guid id);
    Task<UserViewModel?> GetUser(string username);
    Task<User> GetUser(Guid id);
    
}

public class MySqlContext : IMySqlContext
{
    private readonly DbConnection _db;

    public MySqlContext(IConfiguration config)
    {
        _db = new MySqlConnection(config.GetConnectionString("mysql"));
    }

    #region Users
    
    public async Task<List<UserViewModel>> GetUsers()
    {
        return (await _db.QueryAsync<UserViewModel>($@"select 
                    id {nameof(UserViewModel.Id)},
                    username {nameof(UserViewModel.Username)},
                    email {nameof(UserViewModel.Email)},
                    firstname {nameof(UserViewModel.FirstName)},
                    lastname {nameof(UserViewModel.LastName)},
                    created_date {nameof(User.CreatedDate)},
                    updated_date {nameof(User.UpdatedDate)}
                    from Users
                    order by username")).ToList();
    }

    public async Task<UserViewModel?> GetUser(string username)
    {
        return (await _db.QueryAsync<UserViewModel>($@"select 
                    id {nameof(UserViewModel.Id)},
                    username {nameof(UserViewModel.Username)},
                    email {nameof(UserViewModel.Email)},
                    firstname {nameof(UserViewModel.FirstName)},
                    lastname {nameof(UserViewModel.LastName)},
                    created_date {nameof(UserViewModel.CreatedDate)},
                    updated_date {nameof(UserViewModel.UpdatedDate)}
                    from Users where username = @username", new { username = username }))
            .FirstOrDefault();
    }
    
    public async Task<User> GetUser(Guid id)
    {
        return (await _db.QueryAsync<User>($@"select 
                    id {nameof(User.Id)},
                    username {nameof(User.Username)},
                    email {nameof(User.Email)},
                    firstname {nameof(User.FirstName)},
                    lastname {nameof(User.LastName)},
                    salt {nameof(User.Salt)},
                    created_date {nameof(User.CreatedDate)},
                    updated_date {nameof(User.UpdatedDate)}
                    from Users where id = @id", new { id = id }))
            .First();
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
                    id {nameof(User.Id)},
                    username {nameof(User.Username)},
                    email {nameof(User.Email)},
                    firstname {nameof(User.FirstName)},
                    lastname {nameof(User.LastName)},
                    salt {nameof(User.Salt)},
                    created_date {nameof(User.CreatedDate)},
                    updated_date {nameof(User.UpdatedDate)}
                    from Users where id = @id", new { id = user.Id }))
            .First();
    }

    public async Task<bool> UpdateSalt(User user)
    {
        return await _db.ExecuteAsync("update Users set salt = @salt where id = @id", new { id = user.Id, salt = user.Salt }) > 0;
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        return await _db.ExecuteAsync("delete from Users where id = @id", new { id = id }) > 0;
    }
    
    #endregion

}