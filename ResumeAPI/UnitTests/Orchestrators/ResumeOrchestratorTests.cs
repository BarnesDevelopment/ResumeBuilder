using Moq;
using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace UnitTests.Orchestrators;

public class ResumeOrchestratorTests
{
  private readonly Mock<IResumeBuilderService> _service;
  private readonly Mock<IResumeTree> _tree;
  private readonly Mock<IResumeService> _resumeService;
  private readonly ResumeOrchestrator _orchestrator;

  public ResumeOrchestratorTests()
  {
    _service = new Mock<IResumeBuilderService>();
    _tree = new Mock<IResumeTree>();
    _resumeService = new Mock<IResumeService>();
    _orchestrator = new ResumeOrchestrator(_service.Object, _tree.Object, _resumeService.Object);
  }

  [Fact]
  public async Task GetTopLevelResumes_ShouldReturnNodes()
  {
    var userId = Guid.NewGuid();
    var nodes = new List<ResumeTreeNode>
    {
      new()
      {
        Children = null,
        Id = Guid.NewGuid(),
        Active = true,
        UserId = userId,
        ParentId = Guid.Empty,
        Content = "resume1",
        NodeType = "resume",
        Depth = 0,
        Order = 0
      }
    };
    _tree.Setup(x => x.GetTopLevelNodes(userId)).ReturnsAsync(nodes);

    var actual = await _orchestrator.GetTopLevelResumes(userId);

    actual.Should().BeEquivalentTo(nodes);
  }

  [Fact]
  public async Task UpdateNode_ShouldReturnNode()
  {
    var userId = Guid.NewGuid();
    var node = new ResumeTreeNode
    {
      Children = null,
      Id = Guid.NewGuid(),
      Active = true,
      UserId = userId,
      ParentId = Guid.Empty,
      Content = "resume1",
      NodeType = "resume",
      Depth = 0,
      Order = 0
    };

    _tree.Setup(x => x.UpsertNode(node)).ReturnsAsync(node);

    var actual = await _orchestrator.UpsertNode(node, userId);

    actual.Should().BeEquivalentTo(node);
  }

  [Fact]
  public async Task DeleteNode_ShouldReturnSuccess()
  {
    var id = Guid.NewGuid();

    _tree.Setup(x => x.DeleteNode(id)).ReturnsAsync(true);

    var actual = await _orchestrator.DeleteNode(id);

    actual.Should().BeTrue();
  }

  [Fact]
  public async Task DeleteNode_ShouldReturnFailure()
  {
    var id = Guid.NewGuid();

    _tree.Setup(x => x.DeleteNode(id)).ReturnsAsync(false);

    var actual = await _orchestrator.DeleteNode(id);

    actual.Should().BeFalse();
  }

  [Fact]
  public async Task GetResumeTree_ShouldReturnNode()
  {
    var guid1 = Guid.NewGuid();
    var root = new ResumeTreeNode
    {
      Id = guid1,
      Active = true,
      UserId = Guid.Empty,
      ParentId = Guid.Empty,
      Content = "resume1",
      NodeType = "resume",
      Depth = 0,
      Order = 0
    };

    var expected = new ResumeTreeNode
    {
      Id = guid1,
      Active = true,
      UserId = Guid.Empty,
      ParentId = Guid.Empty,
      Content = "resume1",
      NodeType = "resume",
      Depth = 0,
      Order = 0,
      Children = new List<ResumeTreeNode>()
    };

    _tree.Setup(x => x.GetNode(guid1)).ReturnsAsync(root);
    _resumeService.Setup(x => x.GetFullResumeTree(root)).ReturnsAsync(expected);

    var actual = await _orchestrator.GetResumeTree(guid1);

    actual.Should().BeEquivalentTo(expected);
  }
}
