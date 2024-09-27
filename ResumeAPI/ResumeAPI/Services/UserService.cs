using System.Net;
using ResumeAPI.Database;

namespace ResumeAPI.Services;

public interface IUserService
{
    Task<Guid> CreateUser(Guid id);
    Task<string> CreateUser(Guid id, string cookie);
    Task<bool> DeleteUser(string id);
    Task UpdateAccessedDate(Guid id);
    Cookie CreateCookie();
}

public class UserService : IUserService
{
    private readonly IUserData _db;

    public UserService(IUserData db)
    {
        _db = db;
    }

    public async Task<Guid> CreateUser(Guid id) => await _db.CreateUser(id);

    public async Task<string> CreateUser(Guid id, string cookie) => await _db.CreateUser(id, cookie);

    public async Task<bool> DeleteUser(string id) => await _db.DeleteUser(Guid.Parse(id));

    public async Task UpdateAccessedDate(Guid id)
    {
        await _db.UpdateAccessedDate(id);
    }

    public Cookie CreateCookie()
    {
        var cookie = new Cookie("AnonymousUserCookie", Guid.NewGuid().ToString(), "/", "resume-builder.barnes7619.com");
        cookie.Expires = DateTime.Now.AddDays(7);
        cookie.Secure = true;
        return cookie;
    }
}