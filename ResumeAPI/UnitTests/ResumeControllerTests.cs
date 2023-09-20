using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ResumeAPI.Controllers;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace UnitTests;

public class ResumeControllerTests
{
    private readonly ResumeController _controller;
    private readonly Mock<IResumeOrchestrator> _orchestrator;
    private readonly Mock<ILogger<ResumeController>> _logger;
    
    public ResumeControllerTests()
    {
        _orchestrator = new Mock<IResumeOrchestrator>();
        _logger = new Mock<ILogger<ResumeController>>();
        _controller = new ResumeController(_logger.Object,_orchestrator.Object);
    }
    
    [Fact]
    public async Task GetResumeById_ShouldReturnResume()
    {
        var id = Guid.NewGuid();
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
        _orchestrator.Setup(x => x.GetResumeTree(id,"some cookie")).ReturnsAsync(expected);
        
        var actual = (await _controller.GetResumeById(id)).GetObject();
        
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetResumeById_ShouldReturnNotFound()
    {
        var id = Guid.NewGuid();
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
        _orchestrator.Setup(x => x.GetResumeTree(id, "some cookie")).ReturnsAsync((ResumeTreeNode?)null);
        
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
        
        var actual = await _controller.GetResumeById(id);
        
        actual.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetResumeById_ShouldReturnProblem()
    {
        var id = Guid.NewGuid();
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

        _orchestrator.Setup(x => x.GetResumeTree(id, "some cookie")).ThrowsAsync(new Exception("some error"));
        
        var actual = await _controller.GetResumeById(id);
        
        ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
        ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
    }

    [Fact]
    public async Task GetAllResumes_ShouldReturnResumes()
    {
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
        _orchestrator.Setup(x => x.GetTopLevelResumes("some cookie")).ReturnsAsync(expected);
        
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
        
        var actual = (await _controller.GetAllResumes());
        
        actual.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetAllResumes_ShouldReturnProblem()
    {
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
        
        _orchestrator.Setup(x => x.GetTopLevelResumes("some cookie")).ThrowsAsync(new Exception("some error"));
        
        var actual = (await _controller.GetAllResumes());
        
        ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
        ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
    }
}