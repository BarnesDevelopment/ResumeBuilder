using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ResumeAPI.Controllers;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;

namespace UnitTests;

public class ResumeControllerTests
{
    private readonly ResumeController _controller;
    private readonly Mock<IResumeOrchestrator> _orchestrator;
    private readonly Mock<ILogger<ResumeController>> _logger;
    private readonly Mock<ICookieValidator> _cookieValidator;
    
    public ResumeControllerTests()
    {
        _orchestrator = new Mock<IResumeOrchestrator>();
        _logger = new Mock<ILogger<ResumeController>>();
        _cookieValidator = new Mock<ICookieValidator>();
        _controller = new ResumeController(_logger.Object,_orchestrator.Object,_cookieValidator.Object);
    }
    
    #region GetResumeById
    
    [Fact]
    public async Task GetResumeById_ShouldReturnResume()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expected = new ResumeTreeNode{ Id = id };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers = { { "Authorization", "some cookie" } }
                }
            }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.GetResumeTree(id,userId)).ReturnsAsync(expected);
        
        var actual = (await _controller.GetResumeById(id)).GetObject();
        
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetResumeById_ShouldReturnNotFound()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers = { { "Authorization", "some cookie" } }
                }
            }
        };
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.GetResumeTree(id, userId)).ReturnsAsync((ResumeTreeNode?)null);
        
        var actual = await _controller.GetResumeById(id);
        
        actual.Result.Should().BeOfType<NotFoundResult>();
    }
    
    [Fact]
    public async Task GetResumeById_ShouldReturnUnauthorized()
    {
        var id = Guid.NewGuid();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers = { { "Authorization", "" } }
                }
            }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync((Guid?)null);
        
        var actual = await _controller.GetResumeById(id);
        
        actual.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetResumeById_ShouldReturnProblem()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers = { { "Authorization", "some cookie" } }
                }
            }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.GetResumeTree(id, userId)).ThrowsAsync(new Exception("some error"));
        
        var actual = await _controller.GetResumeById(id);
        
        ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
        ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
    }
    
    #endregion
    
    #region GetAllResumes

    [Fact]
    public async Task GetAllResumes_ShouldReturnResumes()
    {
        var userId = Guid.NewGuid();
        var expected = new List<ResumeTreeNode>{ new() };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers = { { "Authorization", "some cookie" } }
                }
            }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.GetTopLevelResumes(userId)).ReturnsAsync(expected);
        
        var actual = (await _controller.GetAllResumes()).GetObject();
        
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllResumes_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers = { { "Authorization", "" } }
                }
            }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync((Guid?)null);
        
        var actual = (await _controller.GetAllResumes());
        
        actual.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetAllResumes_ShouldReturnProblem()
    {
        var userId = Guid.NewGuid();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    Headers = { { "Authorization", "some cookie" } }
                }
            }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.GetTopLevelResumes(userId)).ThrowsAsync(new Exception("some error"));
        
        var actual = (await _controller.GetAllResumes());
        
        ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
        ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
    }
    
    #endregion
    
    #region CreateResume

    [Fact]
    public async Task CreateResume_ReturnsCreatedResume()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var resume = new ResumeTreeNode{ Id = id };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "some cookie" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.CreateResume(resume, userId)).ReturnsAsync(resume);
        
        var actual = (await _controller.CreateResume(resume)).GetObject();
        
        actual.Should().BeEquivalentTo(resume);
    }

    [Fact]
    public async Task CreateResume_ReturnsUnauthorized()
    {
        var id = Guid.NewGuid();
        var resume = new ResumeTreeNode{ Id = id };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync((Guid?)null);
        
        var actual = await _controller.CreateResume(resume);
        
        actual.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task CreateResume_ReturnsProblem()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var resume = new ResumeTreeNode{ Id = id };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "some cookie" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.CreateResume(resume, userId)).ThrowsAsync(new Exception("some error"));
        
        var actual = await _controller.CreateResume(resume);
        
        ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
        ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
    }
    
    #endregion
    
    #region UpdateNode

    [Fact]
    public async Task UpdateNode_ReturnsUpdatedResume()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var resume = new ResumeTreeNode{ Id = id };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "some cookie" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.UpdateNode(resume)).ReturnsAsync(resume);
        
        var actual = (await _controller.UpdateNode(resume)).GetObject();
        
        actual.Should().BeEquivalentTo(resume);
    }

    [Fact]
    public async Task UpdateNode_ReturnsUnauthorized()
    {
        var id = Guid.NewGuid();
        var resume = new ResumeTreeNode{ Id = id };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync((Guid?)null);
        
        var actual = await _controller.UpdateNode(resume);
        
        actual.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task UpdateNode_ReturnsProblem()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var resume = new ResumeTreeNode{ Id = id };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "some cookie" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.UpdateNode(resume)).ThrowsAsync(new Exception("some error"));
        
        var actual = await _controller.UpdateNode(resume);
        
        ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
        ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
    }
    
    #endregion
    
    #region DeleteNode

    [Fact]
    public async Task DeleteNode_ReturnsAccepted()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "some cookie" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.DeleteNode(id)).ReturnsAsync(true);
        
        var actual = (await _controller.DeleteNode(id));
        
        actual.Should().BeOfType<AcceptedResult>();
    }

    [Fact]
    public async Task DeleteNode_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "some cookie" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.DeleteNode(id)).ReturnsAsync(false);
        
        var actual = (await _controller.DeleteNode(id));
        
        actual.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteNode_ReturnsUnauthorized()
    {
        var id = Guid.NewGuid();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync((Guid?)null);
        
        var actual = await _controller.DeleteNode(id);
        
        actual.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task DeleteNode_ReturnsProblem()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { Request = { Headers = { {"Authorization", "some cookie" } } } }
        };
        
        _cookieValidator.Setup(x => x.Validate("some cookie")).ReturnsAsync(userId);
        _orchestrator.Setup(x => x.DeleteNode(id)).ThrowsAsync(new Exception("some error"));
        
        var actual = await _controller.DeleteNode(id);
        
        ((ObjectResult)actual).Value.Should().BeOfType<ProblemDetails>();
        ((ProblemDetails)((ObjectResult)actual).Value!).Detail.Should().Be("some error");
    }
    
    #endregion
}