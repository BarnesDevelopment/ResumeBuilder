using NSubstitute;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;

namespace UnitTests.Orchestrators;

public class DemoOrchestratorTests
{
    private readonly DemoOrchestrator _demoOrchestrator;
    private readonly IResumeOrchestrator _resumeOrchestrator;

    public DemoOrchestratorTests()
    {
        _resumeOrchestrator = Substitute.For<IResumeOrchestrator>();
        _demoOrchestrator = new DemoOrchestrator(_resumeOrchestrator);
    }

    [Fact]
    public async Task InitResumes_WhenCalled_ShouldCallGetTopLevelResumes()
    {
        await _demoOrchestrator.InitResumes(Guid.NewGuid());

        await _resumeOrchestrator.Received(1).GetTopLevelResumes(Guid.Empty);
    }

    [Fact]
    public async Task InitResumes_WhenCalled_ShouldCallDuplicateResumeForEachResume()
    {
        var userId = Guid.NewGuid();
        var resumes = new List<ResumeTreeNode> { new(), new(), new() };
        _resumeOrchestrator.GetTopLevelResumes(Guid.Empty).Returns(resumes);

        await _demoOrchestrator.InitResumes(userId);

        await _resumeOrchestrator.Received(3).DuplicateResume(Arg.Any<ResumeTreeNode>(), userId);
    }
}