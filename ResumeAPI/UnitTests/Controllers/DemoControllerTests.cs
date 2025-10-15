using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ResumeAPI.Controllers;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace UnitTests.Controllers;

public class DemoControllerTests
{
    private readonly DemoController _controller;
    private readonly IDemoOrchestrator _demoOrchestrator;
    private readonly IUserService _userService;

    public DemoControllerTests()
    {
        var logger = Substitute.For<ILogger<DemoController>>();
        _userService = Substitute.For<IUserService>();
        _demoOrchestrator = Substitute.For<IDemoOrchestrator>();
        _controller = new DemoController(_userService, _demoOrchestrator, logger);
    }

    [Fact]
    public async Task Login_ShouldCallCorrectMethods()
    {
        var userId = Guid.NewGuid();

        _userService.CreateAnonymousUser().Returns((jwt: "token", id: userId));

        var actual = (await _controller.Login()).Result as OkObjectResult;

        await _demoOrchestrator.Received().InitResumes(userId);
        actual!.Value.Should().Be("token");
    }

    [Fact]
    public async Task Logout_ShouldCallCorrectMethods()
    {
        var userId = Guid.NewGuid();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new List<ClaimsIdentity>
                {
                    new([new Claim("userId", userId.ToString()), new Claim("isAnonymous", "true")],
                        "Bearer")
                })
            }
        };

        var actual = await _controller.Logout() as NoContentResult;

        actual!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        await _demoOrchestrator.Received().DeleteUser(userId);
        await _userService.Received().DeleteAnonymousUser(userId);
    }

    [Fact]
    public async Task Logout_ShouldReturnUnauthorized_WhenNoUserId()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new List<ClaimsIdentity>
                {
                    new([new Claim("isAnonymous", "true")], "Bearer")
                })
            }
        };

        var actual = await _controller.Logout();
        actual.Should().BeOfType<UnauthorizedResult>();
        await _demoOrchestrator.DidNotReceive().DeleteUser(Arg.Any<Guid>());
        await _userService.DidNotReceive().DeleteAnonymousUser(Arg.Any<Guid>());
    }

    [Fact]
    public async Task Logout_ShouldReturnForbid_WhenNotAnonymous()
    {
        var userId = Guid.NewGuid();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new List<ClaimsIdentity>
                {
                    new([new Claim("userId", userId.ToString()), new Claim("isAnonymous", "false")],
                        "Bearer")
                })
            }
        };

        var actual = await _controller.Logout();
        actual.Should().BeOfType<ForbidResult>();
        await _demoOrchestrator.DidNotReceive().DeleteUser(Arg.Any<Guid>());
        await _userService.DidNotReceive().DeleteAnonymousUser(Arg.Any<Guid>());
    }
}