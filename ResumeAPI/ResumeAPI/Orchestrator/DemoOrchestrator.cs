using ResumeAPI.Services;

namespace ResumeAPI.Orchestrator;

public interface IDemoOrchestrator
{
    Task<string> CreateUser();
    Task DeleteUser(Guid userId);
}

public class DemoOrchestrator(IResumeOrchestrator resumeOrchestrator, IUserService userService)
    : IDemoOrchestrator
{
    public async Task<string> CreateUser()
    {
        var user = await userService.CreateAnonymousUser();

        var resumes = await resumeOrchestrator.GetTopLevelResumes(Guid.Empty);
        foreach (var resume in resumes)
        {
            await resumeOrchestrator.DuplicateResume(resume, user.id);
        }

        return user.jwt;
    }

    public async Task DeleteUser(Guid userId)
    {
        var resumes = await resumeOrchestrator.GetTopLevelResumes(userId);
        foreach (var resume in resumes)
        {
            await resumeOrchestrator.DeleteNode(resume.Id);
        }

        await userService.DeleteAnonymousUser(userId);
    }
}