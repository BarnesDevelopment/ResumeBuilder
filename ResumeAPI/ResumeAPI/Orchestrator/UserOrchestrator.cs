using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace ResumeAPI.Orchestrator;

public interface IUserOrchestrator
{
    Task<LoginAttempt> Login(string username, string key);
    Task<UserViewModel> CreateAccount(UserViewModel userInput, string key);
    Task<bool> DeleteUser(Guid id);
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
            var cookie = await _db.RetrieveCookie(user.IdGuid());
            if (cookie != null) await _db.DeactivateCookie(cookie.KeyGuid());
            cookie = await _db.CreateCookie(user.IdGuid());
            return new LoginAttempt(cookie);
        }

        return new LoginAttempt();
    }

    public async Task<UserViewModel> CreateAccount(UserViewModel userInput, string key)
    {
        var user = await _service.CreateUser(userInput);
        await _service.CreateKey(user.IdGuid(), key);
        return user;
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        await _db.DeleteKeys(id);
        await _db.DeleteCookies(id);
        return await _db.DeleteUser(id);
    }
}