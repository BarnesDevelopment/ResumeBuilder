using ResumeAPI.Database;

namespace ResumeAPI.Services;

public interface IUserService
{
  Task<Guid> CreateUser(Guid id);
  Task<bool> DeleteUser(string id);
}

public class UserService : IUserService
{
  private readonly IUserData _db;

  public UserService(IUserData db)
  {
    _db = db;
  }

  public async Task<Guid> CreateUser(Guid id)
  {
    return await _db.CreateUser(id);
  }

  public async Task<bool> DeleteUser(string id)
  {
    return await _db.DeleteUser(Guid.Parse(id));
  }
}
