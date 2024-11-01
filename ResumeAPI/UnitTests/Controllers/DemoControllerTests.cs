using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ResumeAPI.Controllers;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace UnitTests.Controllers;

public class DemoControllerTests
{
    private readonly DemoController _controller;
    private readonly IDemoOrchestrator _demoOrchestrator;
    private readonly IUserService _userService;
    private readonly IAnonymousUserValidator _validator;

    public DemoControllerTests()
    {
        _validator = Substitute.For<IAnonymousUserValidator>();
        _userService = Substitute.For<IUserService>();
        _demoOrchestrator = Substitute.For<IDemoOrchestrator>();
        _controller = new DemoController(_validator, _userService, _demoOrchestrator);
    }

    [Fact]
    public async Task InitResumes_ShouldCallCorrectMethods()
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

        var actual = await _controller.InitResumes() as OkResult;

        await _userService.Received().GetUser("123");
        await _demoOrchestrator.Received().InitResumes(userId);

        actual.Should().NotBeNull();
    }
}

public class RequestCookieCollection : Dictionary<string, string>, IRequestCookieCollection
{
    public new ICollection<string> Keys => base.Keys;

    public new string? this[string key]
    {
        get
        {
            TryGetValue(key, out var value);
            return value;
        }
        set
        {
            if (value != null) base[key] = value;
        }
    }
}