using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IUserService
{
    Task<List<UserViewModel>> GetUsers();
    Task<UserViewModel?> GetUser(string username);
    Task<User> CreateUser(UserViewModel userInput);
    Task<UserViewModel> UpdateUser(string id, UserViewModel userInput);
    Task<bool> DeleteUser(string id);
    Task<VerificationResult> VerifyKey(Guid id, string key);
}

public class UserService : IUserService
{
    private readonly IMySqlContext _db;
    private readonly IPasswordHasher _hasher;
    
    public UserService(IMySqlContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    #region User

    public async Task<List<UserViewModel>> GetUsers()
    {
        return await _db.GetUsers();
    }

    public async Task<UserViewModel?> GetUser(string username)
    {
        return await _db.GetUser(username);
    }

    public async Task<User> CreateUser(UserViewModel userInput)
    {
        var user = new User
        {
            Username = userInput.Username,
            FirstName = userInput.FirstName,
            LastName = userInput.LastName,
            Email = userInput.Email,
            Id = Guid.NewGuid().ToString(),
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
}