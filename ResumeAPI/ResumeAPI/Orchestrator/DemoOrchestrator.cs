namespace ResumeAPI.Orchestrator;

public interface IDemoOrchestrator
{
    Task InitResumes(Guid userId);
    Task DeleteUser(Guid userId);
}

public class DemoOrchestrator(IResumeOrchestrator resumeOrchestrator, IUserOrchestrator userOrchestrator)
    : IDemoOrchestrator
{
    public async Task InitResumes(Guid userId)
    {
        var resumes = await resumeOrchestrator.GetTopLevelResumes(Guid.Empty);
        foreach (var resume in resumes)
        {
            await resumeOrchestrator.DuplicateResume(resume, userId);
        }
    }

    public async Task DeleteUser(Guid userId)
    {
        var resumes = await resumeOrchestrator.GetTopLevelResumes(userId);
        foreach (var resume in resumes)
        {
            await resumeOrchestrator.DeleteNode(resume.Id);
        }

        await userOrchestrator.DeleteUser(userId);
    }
}