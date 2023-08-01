using ResumeAPI.Database;
using ResumeAPI.Services;

namespace ResumeAPI.Orchestrator;

public interface IUserOrchestrator
{
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
}