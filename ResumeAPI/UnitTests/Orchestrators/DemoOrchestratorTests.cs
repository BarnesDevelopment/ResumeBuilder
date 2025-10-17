using NSubstitute;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace UnitTests.Orchestrators;

public class DemoOrchestratorTests
{
    private readonly DemoOrchestrator _demoOrchestrator;
    private readonly IResumeOrchestrator _resumeOrchestrator;
    private readonly IUserService _userService;

    public DemoOrchestratorTests()
    {
        _resumeOrchestrator = Substitute.For<IResumeOrchestrator>();
        _userService = Substitute.For<IUserService>();
        _demoOrchestrator = new DemoOrchestrator(_resumeOrchestrator, _userService);
    }

    [Fact]
    public async Task InitResumes_WhenCalled_ShouldCallGetTopLevelResumes()
    {
        _userService.CreateAnonymousUser().Returns(("token", Guid.NewGuid()));
        _resumeOrchestrator.GetTopLevelResumes(Guid.Empty).Returns(new List<ResumeTreeNode>());

        var actual = await _demoOrchestrator.CreateUser();

        actual.Should().Be("token");
    }

    [Fact]
    public async Task InitResumes_WhenCalled_ShouldCallDuplicateResumeForEachResume()
    {
        var userId = Guid.NewGuid();
        _userService.CreateAnonymousUser().Returns(("token", userId));
        var resumes = new List<ResumeTreeNode> { new(), new(), new() };
        _resumeOrchestrator.GetTopLevelResumes(Guid.Empty).Returns(resumes);

        await _demoOrchestrator.CreateUser();

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
    public async Task DeleteUser_WhenCalled_ShouldCallDeleteAnonymousUser()
    {
        var userId = Guid.NewGuid();
        var resumes = new List<ResumeTreeNode> { new(), new(), new() };
        _resumeOrchestrator.GetTopLevelResumes(userId).Returns(resumes);

        await _demoOrchestrator.DeleteUser(userId);

        await _userService.Received(1).DeleteAnonymousUser(userId);
    }
}