using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IUserService
{
    Task<List<UserViewModel>> GetUsers();
    Task<UserViewModel?> GetUser(Guid id);
    Task<UserViewModel?> GetUser(string username);
    Task<User> CreateUser(UserViewModel userInput);
    Task<UserViewModel> UpdateUser(string id, UserViewModel userInput);
    Task<bool> DeleteUser(string id);
    Task<bool> CreateKey(Guid id, string key);
    Task<VerificationResult> VerifyKey(Guid id, string key);
    Task<bool> VerifyCookie(Guid id, Guid cookie);
    Task<Guid> GetUserByCookie(Guid cookie);
}

public class UserService : IUserService
{
    private readonly IUserData _db;
    private readonly IPasswordHasher _hasher;
    
    public UserService(IUserData db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    #region User

    public async Task<List<UserViewModel>> GetUsers()
    {
        return await _db.GetUsers();
    }

    public async Task<UserViewModel?> GetUser(Guid id)
    {
        return await _db.GetUser(id);
    }
    
    public async Task<UserViewModel?> GetUser(string username)
    {
        return await _db.GetUser(username);
    }
    
    public async Task<Guid> GetUserByCookie(Guid cookie)
    {
        return await _db.GetUserByCookie(cookie);
    }

    public async Task<User> CreateUser(UserViewModel userInput)
    {
        var user = new User
        {
            Username = userInput.Username,
            FirstName = userInput.FirstName,
            LastName = userInput.LastName,
            Email = userInput.Email,
            Id = Guid.NewGuid(),
        };

        return await _db.CreateUser(user);
    }

    public async Task<UserViewModel> UpdateUser(string id, UserViewModel userInput)
    {
        return await _db.UpdateUser(Guid.Parse(id), userInput);
    }

    public async Task<bool> DeleteUser(string id)
    {
        return await _db.DeleteUser(Guid.Parse(id));
    }

    public async Task<bool> CreateKey(Guid id, string key)
    {
        var hashedKey = _hasher.HashPassword(key);
        return await _db.CreateKey(hashedKey, id);
    }

    #endregion
    
    public async Task<VerificationResult> VerifyKey(Guid id, string key)
    {
        var hash = await _db.RetrieveKey(id);
        if (hash != null)
        {
            if (_hasher.VerifyHashedPassword(hash, key) == PasswordVerificationResult.Success) return VerificationResult.Correct;
            return VerificationResult.Incorrect;
        }

        return VerificationResult.NotFound;
    }

    public async Task<bool> VerifyCookie(Guid id, Guid cookie)
    {
        var userCookie = await _db.RetrieveCookie(id);
        if (userCookie != null) return cookie == userCookie.Key && userCookie.Expiration > DateTime.Now;
        return false;
    }
}