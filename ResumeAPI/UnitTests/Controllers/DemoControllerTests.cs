using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ResumeAPI.Controllers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;
using UnitTests.Models;

namespace UnitTests.Controllers;

public class DemoControllerTests
{
    private readonly DemoController _controller;
    private readonly IDemoOrchestrator _demoOrchestrator;
    private readonly IUserOrchestrator _userOrchestrator;
    private readonly IUserService _userService;

    public DemoControllerTests()
    {
        var logger = Substitute.For<ILogger<DemoController>>();
        _userOrchestrator = Substitute.For<IUserOrchestrator>();
        _userService = Substitute.For<IUserService>();
        _demoOrchestrator = Substitute.For<IDemoOrchestrator>();
        _controller = new DemoController(_userOrchestrator, _userService, _demoOrchestrator, logger);
    }

    [Fact]
    public async Task Login_ShouldCallCorrectMethods()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        _userService.GetUser("123").Returns(user);
        _userOrchestrator.GetNewCookie().Returns(new Cookie("resume-id", "123"));
        var actual = (await _controller.Login()).Result as OkObjectResult;

        await _userService.Received().GetUser("123");
        await _demoOrchestrator.Received().InitResumes(userId);

        actual!.Value.Should().BeEquivalentTo(new Cookie("resume-id", "123"));
    }

    [Fact]
    public async Task Logout_ShouldCallCorrectMethods()
    {
        var userId = Guid.NewGuid();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request = { Cookies = new RequestCookieCollection { { "resume-id", "123" } } }
            }
        };

        var user = new User { Id = userId };
        _userService.GetUser("123").Returns(user);

        var actual = await _controller.Logout() as NoContentResult;

        await _demoOrchestrator.Received().DeleteUser(userId);
        actual!.Should().NotBeNull();
    }
}