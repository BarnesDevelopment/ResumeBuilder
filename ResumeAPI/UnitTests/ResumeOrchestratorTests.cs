using Moq;
using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace UnitTests;

public class ResumeOrchestratorTests
{
    private readonly Mock<IResumeService> _service;
    private readonly Mock<IResumeTree> _tree;
    private readonly ResumeOrchestrator _orchestrator;
    
    public ResumeOrchestratorTests()
    {
        _service = new Mock<IResumeService>();
        _tree = new Mock<IResumeTree>();
        _orchestrator = new ResumeOrchestrator(_service.Object, _tree.Object);
    }

    [Fact]
    public async Task GetTopLevelResumes_ShouldReturnNodes()
    {
        var userId = Guid.NewGuid();
        var nodes = new List<ResumeTreeNode>
        {
            new ResumeTreeNode
            {
                Children = null,
                Id = Guid.NewGuid(),
                Active = true,
                UserId = userId,
                ParentId = Guid.Empty,
                Content = "resume1",
                SectionType = "resume",
                Depth = 0,
                Order = 0,

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
            SectionType = "resume",
            Depth = 0,
            Order = 0,

        };
        
        _tree.Setup(x => x.UpdateNode(node)).ReturnsAsync(node);
        
        var actual = await _orchestrator.UpdateNode(node);
        
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
    public async Task CreateResume_ShouldReturnNode()
    {
        var userId = Guid.NewGuid();
        var node = new ResumeTreeNode
        {
            Id = Guid.NewGuid(),
            Active = true,
            UserId = userId,
            ParentId = Guid.Empty,
            Content = "resume1",
            SectionType = "resume",
            Depth = 0,
            Order = 0,

        };
        
        _tree.Setup(x => x.CreateNode(node)).ReturnsAsync(true);
        
        var actual = await _orchestrator.CreateResume(node, userId);
        
        actual.Should().BeEquivalentTo(node);
    }

    [Fact] 
    public async Task CreateResume_ShouldReturnTree()
    {
        var userId = Guid.NewGuid();
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();
        var guid4 = Guid.NewGuid();
        var node = new ResumeTreeNode
        {
            Children = new List<ResumeTreeNode>
            {
                new ResumeTreeNode
                {
                    Id = guid2,
                    Active = true,
                    UserId = Guid.Empty,
                    ParentId = guid1,
                    Content = "resume2",
                    SectionType = ResumeSectionType.Section,
                    Depth = 1,
                    Order = 0,

                },
                new ResumeTreeNode
                {
                    Children = new List<ResumeTreeNode>
                    {
                        new ResumeTreeNode
                        {
                            Id = guid4,
                            Active = true,
                            UserId = Guid.Empty,
                            ParentId = guid3,
                            Content = "resume4",
                            SectionType = ResumeSectionType.Paragraph,
                            Depth = 2,
                            Order = 0,

                        },
                    },
                    Id = guid3,
                    Active = true,
                    UserId = Guid.Empty,
                    ParentId = guid1,
                    Content = "resume3",
                    SectionType = ResumeSectionType.Section,
                    Depth = 1,
                    Order = 1,

                },
            },
            Id = guid1,
            Active = true,
            UserId = Guid.Empty,
            ParentId = Guid.Empty,
            Content = "resume1",
            SectionType = "resume",
            Depth = 0,
            Order = 0,
        };
        
        var expected = new ResumeTreeNode
        {
            Children = new List<ResumeTreeNode>
            {
                new ResumeTreeNode
                {
                    Id = guid2,
                    Active = true,
                    UserId = userId,
                    ParentId = guid1,
                    Content = "resume2",
                    SectionType = ResumeSectionType.Section,
                    Depth = 1,
                    Order = 0,

                },
                new ResumeTreeNode
                {
                    Children = new List<ResumeTreeNode>
                    {
                        new ResumeTreeNode
                        {
                            Id = guid4,
                            Active = true,
                            UserId = userId,
                            ParentId = guid3,
                            Content = "resume4",
                            SectionType = ResumeSectionType.Paragraph,
                            Depth = 2,
                            Order = 0,

                        },
                    },
                    Id = guid3,
                    Active = true,
                    UserId = userId,
                    ParentId = guid1,
                    Content = "resume3",
                    SectionType = ResumeSectionType.Section,
                    Depth = 1,
                    Order = 1,

                },
            },
            Id = guid1,
            Active = true,
            UserId = userId,
            ParentId = Guid.Empty,
            Content = "resume1",
            SectionType = "resume",
            Depth = 0,
            Order = 0,
        };
        
        _tree.Setup(x => x.CreateNode(node)).ReturnsAsync(true);
        _tree.Setup(x => x.CreateNode(node.Children[0])).ReturnsAsync(true);
        _tree.Setup(x => x.CreateNode(node.Children[1])).ReturnsAsync(true);
        _tree.Setup(x => x.CreateNode(node.Children[1].Children[0])).ReturnsAsync(true);
        
        var actual = await _orchestrator.CreateResume(node, userId);
        
        actual.Should().BeEquivalentTo(expected);
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
            SectionType = "resume",
            Depth = 0,
            Order = 0,
        };
        
        var expected = new ResumeTreeNode
        {
            Id = guid1,
            Active = true,
            UserId = Guid.Empty,
            ParentId = Guid.Empty,
            Content = "resume1",
            SectionType = "resume",
            Depth = 0,
            Order = 0,
            Children = new List<ResumeTreeNode>()
        };
        
        _tree.Setup(x => x.GetNode(guid1)).ReturnsAsync(root);
        _tree.Setup(x => x.GetChildren(guid1)).ReturnsAsync(new List<ResumeTreeNode>());
        
        var actual = await _orchestrator.GetResumeTree(guid1);
        
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetResumeTree_ShouldReturnTree()
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
            SectionType = "resume",
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
                SectionType = ResumeSectionType.Section,
                Depth = 1,
                Order = 0,
            },
            new()
            {
                Id = guid3,
                Active = true,
                UserId = userId,
                ParentId = guid1,
                Content = "resume3",
                SectionType = ResumeSectionType.Section,
                Depth = 1,
                Order = 1,
            }
        };

        var children2 = new List<ResumeTreeNode>
        {
            new ResumeTreeNode
            {
                Id = guid4,
                Active = true,
                UserId = userId,
                ParentId = guid3,
                Content = "resume4",
                SectionType = ResumeSectionType.Paragraph,
                Depth = 2,
                Order = 0,
                Children = new List<ResumeTreeNode>()
            },
        };
        
        var expected = new ResumeTreeNode
        {
            Children = new List<ResumeTreeNode>
            {
                new ResumeTreeNode
                {
                    Id = guid2,
                    Active = true,
                    UserId = userId,
                    ParentId = guid1,
                    Content = "resume2",
                    SectionType = ResumeSectionType.Section,
                    Depth = 1,
                    Order = 0,
                    Children = new List<ResumeTreeNode>()
                },
                new ResumeTreeNode
                {
                    Children = new List<ResumeTreeNode>
                    {
                        new ResumeTreeNode
                        {
                            Id = guid4,
                            Active = true,
                            UserId = userId,
                            ParentId = guid3,
                            Content = "resume4",
                            SectionType = ResumeSectionType.Paragraph,
                            Depth = 2,
                            Order = 0,
                            Children = new List<ResumeTreeNode>()
                        },
                    },
                    Id = guid3,
                    Active = true,
                    UserId = userId,
                    ParentId = guid1,
                    Content = "resume3",
                    SectionType = ResumeSectionType.Section,
                    Depth = 1,
                    Order = 1,

                },
            },
            Id = guid1,
            Active = true,
            UserId = userId,
            ParentId = Guid.Empty,
            Content = "resume1",
            SectionType = "resume",
            Depth = 0,
            Order = 0,
        };
        
        _tree.Setup(x => x.GetNode(guid1)).ReturnsAsync(root);
        _tree.Setup(x => x.GetChildren(guid1)).ReturnsAsync(children1);
        _tree.Setup(x => x.GetChildren(guid2)).ReturnsAsync(new List<ResumeTreeNode>());
        _tree.Setup(x => x.GetChildren(guid3)).ReturnsAsync(children2);
        _tree.Setup(x => x.GetChildren(guid4)).ReturnsAsync(new List<ResumeTreeNode>());
        
        var actual = await _orchestrator.GetResumeTree(guid1);
        
        actual.Should().BeEquivalentTo(expected);
    }
}