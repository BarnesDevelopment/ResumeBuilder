using System.Net;
using ResumeAPI.Database;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IUserService
{
    Task<User?> GetUser(string cookie);
    Task<Guid> CreateUser(Guid id);
    Task<bool> DeleteUser(Guid id);
    Task UpdateAccessedDate(Guid id);
    Cookie CreateCookie();
    Task<Cookie?> GetCookie(string cookie);
    Task DeleteCookie(string cookie);
    Task<string> CreateAnonymousUser();
}

public class UserService : IUserService
{
    private readonly IUserData _db;
    private readonly IAuthClient _authClient;

    public UserService(IUserData db, IAuthClient authClient)
    {
        _db = db;
        _authClient = authClient;
    }

    public Task<User?> GetUser(string cookie) => _db.GetUser(cookie);

    public async Task<Guid> CreateUser(Guid id) => await _db.CreateUser(id);

    public async Task<bool> DeleteUser(Guid id) => await _db.DeleteUser(id);

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

    public async Task DeleteCookie(string cookie)
    {
        var user = await _db.GetUser(cookie);
        if (user == null) return;
        await _db.DeleteUser(user.Id);
    }

    public async Task<string> CreateAnonymousUser()
    {
        var id = await _authClient.CreateAnonymousUser();
        if (id == null) throw new Exception("Failed to create anonymous user");
        return await _authClient.VendJwtFromId(id.Value);
    }
}