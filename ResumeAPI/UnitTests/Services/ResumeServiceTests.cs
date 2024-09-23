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
