using System.Data.Common;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public interface IMySqlContext
{
    Task<List<UserViewModel>> GetUsers();
    Task<User> CreateUser(User user);
    Task<UserViewModel> UpdateUser(Guid id,UserViewModel user);
    Task<bool> DeleteUser(Guid id);
    Task<UserViewModel?> GetUser(string username);
    Task<User> GetUser(Guid id);
    Task<bool> CreateKey(string hash, Guid userId);
    Task<string?> RetrieveKey(Guid userId);
    Task<bool> DeactivateKey(Guid userId);
    Task<bool> DeleteKeys(Guid userId);
    Task<Cookie> CreateCookie(Guid userId);
    Task DeactivateCookie(Guid cookie);
    Task<Cookie?> RetrieveCookie(Guid userId);
    Task<bool> DeleteCookies(Guid userId);
}

public class MySqlContext : IMySqlContext
{
    private readonly DbConnection _db;

    public MySqlContext(IOptions<AWSSecrets> options)
    {
        var secrets = options.Value;
        _db = new MySqlConnection(secrets.ConnectionStrings_MySql);
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
                    lastname {nameof(UserViewModel.LastName)}
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
                    created_date {nameof(User.CreatedDate)},
                    updated_date {nameof(User.UpdatedDate)}
                    from Users where id = @id", new { id = id }))
            .First();
    }

    public async Task<User> CreateUser(User user)
    {
        await _db.ExecuteAsync(
            @"insert into Users (id,username,email,firstname,lastname) values(@id,@username,@email,@firstname,@lastname)",
            new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                firstname = user.FirstName,
                lastname = user.LastName,
            });

        return (await _db.QueryAsync<User>($@"select 
                    id {nameof(User.Id)},
                    username {nameof(User.Username)},
                    email {nameof(User.Email)},
                    firstname {nameof(User.FirstName)},
                    lastname {nameof(User.LastName)},
                    created_date {nameof(User.CreatedDate)},
                    updated_date {nameof(User.UpdatedDate)}
                    from Users where id = @id", new { id = user.Id }))
            .First();
    }
    
    public async Task<UserViewModel> UpdateUser(Guid id, UserViewModel user)
    {
        await _db.ExecuteAsync(
            @"update Users set username = @username, email = @email, firstname = @firstname, lastname = @lastname, updated_date = now() where id = @id",
            new
            {
                id = id,
                username = user.Username,
                email = user.Email,
                firstname = user.FirstName,
                lastname = user.LastName
            });

        return (await _db.QueryAsync<UserViewModel>($@"select 
                    id {nameof(UserViewModel.Id)},
                    username {nameof(UserViewModel.Username)},
                    email {nameof(UserViewModel.Email)},
                    firstname {nameof(UserViewModel.FirstName)},
                    lastname {nameof(UserViewModel.LastName)}
                    from Users where id = @id", new { id = id }))
            .First();
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        return await _db.ExecuteAsync("delete from Users where id = @id", new { id = id }) > 0;
    }
    
    #endregion

    #region Keys

    public async Task<bool> CreateKey(string hash, Guid userId)
    {
        return await _db.ExecuteAsync("insert into PasswordHashes (id,userid,hash) values(@id,@userid,@hash)", new
        {
            id = Guid.NewGuid(),
            userid = userId,
            hash = hash
        }) > 0;
    }

    public async Task<string?> RetrieveKey(Guid userId)
    {
        return (await _db.QueryAsync<string>(
            "select hash from PasswordHashes where userid = @userid and active = true order by created_date", new { userid = userId})).FirstOrDefault();
    }

    public async Task<bool> DeleteKeys(Guid userId)
    {
        return await _db.ExecuteAsync("delete from PasswordHashes where userid = @userid",
            new { userid = userId }) > 0;
    }

    public async Task<bool> DeactivateKey(Guid userId)
    {
        return await _db.ExecuteAsync("update PasswordHashes set active = false where userid = @userid",
            new { userid = userId }) > 0;
    }

    #endregion

    #region Cookies

    public async Task<Cookie> CreateCookie(Guid userId)
    {
        var cookie = Guid.NewGuid();
        await _db.ExecuteAsync("insert into Cookies (cookie, expiration, userid) values(@cookie, @expiration, @userid)", new
        {
            cookie = cookie,
            expiration = DateTime.Now.AddDays(1),
            userid = userId
        });

        return (await _db.QueryAsync<Cookie>(
            $@"select cookie `{nameof(Cookie.Key)}`, 
                    active {nameof(Cookie.Active)}, 
                    expiration {nameof(Cookie.Expiration)}, 
                    userid {nameof(Cookie.UserId)}
                    from Cookies where cookie = @cookie", new
            {
                cookie = cookie
            })).First();
    }

    public async Task DeactivateCookie(Guid cookie)
    {
        await _db.ExecuteAsync("update Cookies set active = false where cookie = @cookie", new { cookie = cookie });
    }

    public async Task<Cookie?> RetrieveCookie(Guid userId)
    {
        return (await _db.QueryAsync<Cookie>($@"select cookie `{nameof(Cookie.Key)}`, 
                    active {nameof(Cookie.Active)}, 
                    expiration {nameof(Cookie.Expiration)}, 
                    userid {nameof(Cookie.UserId)}
                    from Cookies where userid = @userid and active = true", new
        {
            userid = userId
        })).FirstOrDefault();
    }

    public async Task<bool> DeleteCookies(Guid userId)
    {
        return await _db.ExecuteAsync("delete from Cookies where userid = @userid",
            new { userid = userId }) > 0;
    }

    #endregion
}