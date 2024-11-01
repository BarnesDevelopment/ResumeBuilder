namespace ResumeAPI.Orchestrator;

public interface IDemoOrchestrator
{
    Task InitResumes(Guid userId);
}

public class DemoOrchestrator : IDemoOrchestrator
{
    private readonly IResumeOrchestrator _resumeOrchestrator;

    public DemoOrchestrator(IResumeOrchestrator resumeOrchestrator)
    {
        _resumeOrchestrator = resumeOrchestrator;
    }

    public async Task InitResumes(Guid userId)
    {
        var resumes = await _resumeOrchestrator.GetTopLevelResumes(Guid.Empty);
        foreach (var resume in resumes)
        {
            await _resumeOrchestrator.DuplicateResume(resume, userId);
        }
    }
}