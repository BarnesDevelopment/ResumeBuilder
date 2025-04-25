using NSubstitute;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;

namespace UnitTests.Orchestrators;

public class DemoOrchestratorTests
{
    private readonly DemoOrchestrator _demoOrchestrator;
    private readonly IResumeOrchestrator _resumeOrchestrator;
    private readonly IUserOrchestrator _userOrchestrator;

    public DemoOrchestratorTests()
    {
        _resumeOrchestrator = Substitute.For<IResumeOrchestrator>();
        _userOrchestrator = Substitute.For<IUserOrchestrator>();
        _demoOrchestrator = new DemoOrchestrator(_resumeOrchestrator, _userOrchestrator);
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

    [Fact]
    public async Task DeleteUser_WhenCalled_ShouldCallGetTopLevelResumes()
    {
        var userId = Guid.NewGuid();
        await _demoOrchestrator.DeleteUser(userId);

        await _resumeOrchestrator.Received(1).GetTopLevelResumes(userId);
    }

    [Fact]
    public async Task DeleteUser_WhenCalled_ShouldCallDeleteNodeForEachResume()
    {
        var userId = Guid.NewGuid();
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var resumes = new List<ResumeTreeNode> { new() { Id = ids[0] }, new() { Id = ids[1] }, new() { Id = ids[2] } };
        _resumeOrchestrator.GetTopLevelResumes(userId).Returns(resumes);

        await _demoOrchestrator.DeleteUser(userId);

        await _resumeOrchestrator.Received(1).DeleteNode(ids[0]);
        await _resumeOrchestrator.Received(1).DeleteNode(ids[1]);
        await _resumeOrchestrator.Received(1).DeleteNode(ids[2]);
    }

    [Fact]
    public async Task DeleteUser_WhenCalled_ShouldCallDeleteUser()
    {
        var userId = Guid.NewGuid();
        var resumes = new List<ResumeTreeNode> { new(), new(), new() };
        _resumeOrchestrator.GetTopLevelResumes(userId).Returns(resumes);

        await _demoOrchestrator.DeleteUser(userId);

        await _userOrchestrator.Received(1).DeleteUser(userId);
    }
}