using System.Net;
using Npgsql;
using ResumeAPI.Database;
using ResumeAPI.Services;

namespace ResumeAPI.Orchestrator;

public interface IUserOrchestrator
{
    Task<Guid> CreateUser();
    Task<bool> DeleteUser(Guid id);
    Task<Cookie> GetNewCookie();
}

public class UserOrchestrator : IUserOrchestrator
{
    private readonly IUserData _db;
    private readonly IUserService _service;

    public UserOrchestrator(IUserService service, IUserData db)
    {
        _db = db;
        _service = service;
    }

    public async Task<Guid> CreateUser() => await _service.CreateUser(Guid.NewGuid());

    public async Task<bool> DeleteUser(Guid id) => await _db.DeleteUser(id);

    public async Task<Cookie> GetNewCookie()
    {
        var cookie = _service.CreateCookie();
        var id = await _db.CreateUser(Guid.NewGuid(), cookie);
        if (id == Guid.Empty) throw new NpgsqlException("Failed to create user");
        return cookie;
    }
}