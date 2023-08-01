using ResumeAPI.Database;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IUserService
{
    Task<List<UserViewModel>> GetUsers();
    Task<UserViewModel?> GetUser(string username);
    Task<User> CreateUser(UserViewModel userInput);
    Task<UserViewModel> UpdateUser(string id, UserViewModel userInput);
    Task<bool> DeleteUser(string id);
}

public class UserService : IUserService
{
    private readonly IMySqlContext _db;
    
    public UserService(IMySqlContext db)
    {
        _db = db;
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
}