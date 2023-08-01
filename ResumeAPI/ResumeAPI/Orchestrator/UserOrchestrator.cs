using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace ResumeAPI.Orchestrator;

public interface IUserOrchestrator
{
    Task<LoginAttempt> Login(string username, string key);
}

public class UserOrchestrator : IUserOrchestrator
{
    private readonly IMySqlContext _db;   
    private readonly IUserService _service;

    public UserOrchestrator(IUserService service, IMySqlContext db)
    {
        _db = db;
        _service = service;
    }

    public async Task<LoginAttempt> Login(string username, string key)
    {
        var user = await _service.GetUser(username);
        if (user == null) return new LoginAttempt();
        var verified = await _service.VerifyKey(user.IdGuid(), key);
        if (verified == VerificationResult.Correct)
        {
            var cookie = await _db.CreateCookie(user.IdGuid());
            return new LoginAttempt(cookie);
        }

        return new LoginAttempt();
    }
}