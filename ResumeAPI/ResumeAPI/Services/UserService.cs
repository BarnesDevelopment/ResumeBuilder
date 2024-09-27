using System.Net;
using ResumeAPI.Database;

namespace ResumeAPI.Services;

public interface IUserService
{
    Task<Guid> CreateUser(Guid id);
    Task<bool> DeleteUser(string id);
    Task UpdateAccessedDate(Guid id);
    Cookie CreateCookie();
    Task<Cookie?> GetCookie(string cookie);
}

public class UserService : IUserService
{
    private readonly IUserData _db;

    public UserService(IUserData db)
    {
        _db = db;
    }

    public async Task<Guid> CreateUser(Guid id) => await _db.CreateUser(id);

    public async Task<bool> DeleteUser(string id) => await _db.DeleteUser(Guid.Parse(id));

    public async Task UpdateAccessedDate(Guid id)
    {
        await _db.UpdateAccessedDate(id);
    }

    public Cookie CreateCookie()
    {
        var cookie = new Cookie("AnonymousUserCookie", Guid.NewGuid().ToString(), "/", "resume-builder.barnes7619.com")
        {
            Expires = DateTime.Now.AddDays(7), Secure = true
        };
        return cookie;
    }

    public async Task<Cookie?> GetCookie(string cookie)
    {
        var user = await _db.GetUser(cookie);
        if (user == null) return null;
        return new Cookie("AnonymousUserCookie", user.Cookie, "/", "resume-builder.barnes7619.com")
        {
            Expires = user.CookieExpiration, Secure = true
        };
    }
}