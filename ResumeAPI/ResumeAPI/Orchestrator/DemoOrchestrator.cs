namespace ResumeAPI.Orchestrator;

public interface IDemoOrchestrator
{
    Task InitResumes(Guid userId);
    Task DeleteUser(Guid userId);
}

public class DemoOrchestrator : IDemoOrchestrator
{
    private readonly IResumeOrchestrator _resumeOrchestrator;
    private readonly IUserOrchestrator _userOrchestrator;

    public DemoOrchestrator(IResumeOrchestrator resumeOrchestrator, IUserOrchestrator userOrchestrator)
    {
        _resumeOrchestrator = resumeOrchestrator;
        _userOrchestrator = userOrchestrator;
    }

    public async Task InitResumes(Guid userId)
    {
        var resumes = await _resumeOrchestrator.GetTopLevelResumes(Guid.Empty);
        foreach (var resume in resumes)
        {
            await _resumeOrchestrator.DuplicateResume(resume, userId);
        }
    }

    public async Task DeleteUser(Guid userId)
    {
        var resumes = await _resumeOrchestrator.GetTopLevelResumes(userId);
        foreach (var resume in resumes)
        {
            await _resumeOrchestrator.DeleteNode(resume.Id);
        }

        await _userOrchestrator.DeleteUser(userId);
    }
}