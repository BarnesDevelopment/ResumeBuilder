using System.Net;
using Dapper;
using Microsoft.Extensions.Options;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public interface IUserData
{
    Task<User?> GetUser(Guid id);
    Task<User?> GetUser(string cookie);
    Task<Guid> CreateUser(Guid id);
    Task<Guid> CreateUser(Guid id, Cookie cookie);
    Task<bool> DeleteUser(Guid id);
    Task UpdateAccessedDate(Guid id);
    Task<DateTime> UpdateCookieExpiration(Guid id);
}

public class UserData : PostgreSqlContext, IUserData
{
    private const string UserSelect = @$"
                    id {nameof(User.Id)},
                    created_date {nameof(User.CreatedDate)},
                    demo {nameof(User.Demo)},
                    demo_session_cookie {nameof(User.Cookie)},
                    cookie_expiration {nameof(User.CookieExpiration)}";

    public UserData(IOptions<AWSSecrets> options) : base(options) { }

    public async Task<User?> GetUser(Guid id) => (await Db.QueryAsync<User>($@"select 
                    {UserSelect}
                    from ResumeDb.Users where id = @id",
            new { id }))
        .FirstOrDefault();

    public async Task<User?> GetUser(string cookie) => (await Db.QueryAsync<User>($@"select {UserSelect}
                    from ResumeDb.Users
                    where demo_session_cookie = @cookie",
            new { cookie }))
        .FirstOrDefault();

    public async Task<Guid> CreateUser(Guid id)
    {
        await Db.ExecuteAsync(
            @"insert into ResumeDb.Users (id) values(@id)",
            new { id });

        return id;
    }

    public async Task<Guid> CreateUser(Guid id, Cookie cookie) => await Db.QuerySingleAsync<Guid>(
        @"insert into ResumeDb.Users (id, demo, demo_session_cookie, cookie_expiration) values(@id, true, @cookieValue, @expiration) returning id",
        new { id, cookieValue = cookie.Value, expiration = cookie.Expires });

    public async Task<bool> DeleteUser(Guid id) =>
        await Db.ExecuteAsync("delete from ResumeDb.Users where id = @id", new { id }) > 0;

    public async Task UpdateAccessedDate(Guid id)
    {
        await Db.ExecuteAsync("update ResumeDb.Users set last_accessed = now() where id = @id", new { id });
    }

    public async Task<DateTime> UpdateCookieExpiration(Guid id) => await Db.QuerySingleAsync<DateTime>(
        "update ResumeDb.Users set cookie_expiration = now() + interval '7 days' where id = @id returning cookie_expiration",
        new { id });
}