using ResumeAPI.Database;
using ResumeAPI.Services;

namespace ResumeAPI.Orchestrator;

public interface IUserOrchestrator
{
  Task<Guid> CreateUser();
  Task<bool> DeleteUser(Guid id);
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

  public async Task<Guid> CreateUser()
  {
    return await _service.CreateUser(Guid.NewGuid());
  }

  public async Task<bool> DeleteUser(Guid id)
  {
    return await _db.DeleteUser(id);
  }
}
