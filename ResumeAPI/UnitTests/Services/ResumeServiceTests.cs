using NSubstitute;
using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace UnitTests.Services;

public class ResumeServiceTests
{
  private readonly IResumeTree _tree;
  private readonly IResumeService _service;

  public ResumeServiceTests()
  {
    _tree = Substitute.For<IResumeTree>();
    _service = new ResumeService(_tree);
  }

  [Fact]
  public async Task DuplicateResume_ShouldReturnNewGuid()
  {
    var originalGuid = Guid.NewGuid();
    var root = new ResumeTreeNode()
    {
      Id = originalGuid
    };

    _tree.UpsertNode(Arg.Any<ResumeTreeNode>()).Returns(root);

    var actual = await _service.DuplicateResume(root);

    actual.Should().NotBe(originalGuid);
  }

  [Fact]
  public async Task DuplicateResume_ShouldCallUpsertNode()
  {
    var originalGuid = Guid.NewGuid();
    var root = new ResumeTreeNode()
    {
      Id = originalGuid
    };

    _tree.UpsertNode(Arg.Any<ResumeTreeNode>()).Returns(root);

    await _service.DuplicateResume(root);

    await _tree.Received(1).UpsertNode(Arg.Is<ResumeTreeNode>(n => n.Id != originalGuid));
  }

  [Fact]
  public async Task DuplicateResume_ShouldGiveChildrenNewIds()
  {
    var originalGuids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
    var root = new ResumeTreeNode()
    {
      Id = originalGuids[0],
      Content = "root",
      Children = new List<ResumeTreeNode>
      {
        new() { Id = originalGuids[1],Content = "child1" },
        new() { Id = originalGuids[2], Content = "child2" },
        new() { Id = originalGuids[3],Content = "child3", Children = new List<ResumeTreeNode> { new() { Id = originalGuids[4], Content = "grandchild"} } }
      }
    };

    _tree.UpsertNode(Arg.Any<ResumeTreeNode>()).Returns(root, root.Children[0], root.Children[1], root.Children[2],
      root.Children[2].Children[0]);

    await _service.DuplicateResume(root);

    await _tree.Received(1).UpsertNode(Arg.Is<ResumeTreeNode>(n => n.Id != originalGuids[0] && n.Content == "root"));
    await _tree.Received(1).UpsertNode(Arg.Is<ResumeTreeNode>(n => n.Id != originalGuids[1] && n.Content == "child1"));
    await _tree.Received(1).UpsertNode(Arg.Is<ResumeTreeNode>(n => n.Id != originalGuids[2] && n.Content == "child2"));
    await _tree.Received(1).UpsertNode(Arg.Is<ResumeTreeNode>(n => n.Id != originalGuids[3] && n.Content == "child3"));
    await _tree.Received(1).UpsertNode(Arg.Is<ResumeTreeNode>(n => n.Id != originalGuids[4] && n.Content == "grandchild"));
  }

  [Fact]
  public async Task GetFullResumeTree_ShouldReturnTree()
  {
    var userId = Guid.NewGuid();
    var guid1 = Guid.NewGuid();
    var guid2 = Guid.NewGuid();
    var guid3 = Guid.NewGuid();
    var guid4 = Guid.NewGuid();

    var root = new ResumeTreeNode
    {
      Id = guid1,
      Active = true,
      UserId = userId,
      ParentId = Guid.Empty,
      Content = "resume1",
      NodeType = "resume",
      Depth = 0,
      Order = 0,
      Children = new List<ResumeTreeNode>()
    };

    var children1 = new List<ResumeTreeNode>
    {
      new()
      {
        Id = guid2,
        Active = true,
        UserId = userId,
        ParentId = guid1,
        Content = "resume2",
        NodeType = ResumeNodeType.Section,
        Depth = 1,
        Order = 0
      },
      new()
      {
        Id = guid3,
        Active = true,
        UserId = userId,
        ParentId = guid1,
        Content = "resume3",
        NodeType = ResumeNodeType.Section,
        Depth = 1,
        Order = 1
      }
    };

    var children2 = new List<ResumeTreeNode>
    {
      new()
      {
        Id = guid4,
        Active = true,
        UserId = userId,
        ParentId = guid3,
        Content = "resume4",
        NodeType = ResumeNodeType.Paragraph,
        Depth = 2,
        Order = 0,
        Children = new List<ResumeTreeNode>()
      }
    };

    var expected = new ResumeTreeNode
    {
      Children = new List<ResumeTreeNode>
      {
        new()
        {
          Id = guid2,
          Active = true,
          UserId = userId,
          ParentId = guid1,
          Content = "resume2",
          NodeType = ResumeNodeType.Section,
          Depth = 1,
          Order = 0,
          Children = new List<ResumeTreeNode>()
        },
        new()
        {
          Children = new List<ResumeTreeNode>
          {
            new()
            {
              Id = guid4,
              Active = true,
              UserId = userId,
              ParentId = guid3,
              Content = "resume4",
              NodeType = ResumeNodeType.Paragraph,
              Depth = 2,
              Order = 0,
              Children = new List<ResumeTreeNode>()
            }
          },
          Id = guid3,
          Active = true,
          UserId = userId,
          ParentId = guid1,
          Content = "resume3",
          NodeType = ResumeNodeType.Section,
          Depth = 1,
          Order = 1
        }
      },
      Id = guid1,
      Active = true,
      UserId = userId,
      ParentId = Guid.Empty,
      Content = "resume1",
      NodeType = "resume",
      Depth = 0,
      Order = 0
    };

    _tree.GetChildren(guid1).Returns(children1);
    _tree.GetChildren(guid2).Returns(new List<ResumeTreeNode>());
    _tree.GetChildren(guid3).Returns(children2);
    _tree.GetChildren(guid4).Returns(new List<ResumeTreeNode>());

    var actual = await _service.GetFullResumeTree(root);

    actual.Should().BeEquivalentTo(expected);
  }
}
