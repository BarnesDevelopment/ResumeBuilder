using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/demo")]
public class DemoController : ControllerBase
{
    private readonly IDemoOrchestrator _demoOrchestrator;
    private readonly ILogger<DemoController> _logger;
    private readonly IUserOrchestrator _userOrchestrator;
    private readonly IUserService _userService;

    public DemoController(
        IUserOrchestrator userOrchestrator,
        IUserService userService,
        IDemoOrchestrator demoOrchestrator,
        ILogger<DemoController> logger
    )
    {
        _userOrchestrator = userOrchestrator;
        _userService = userService;
        _demoOrchestrator = demoOrchestrator;
        _logger = logger;
    }

    [HttpPut("login")]
    public async Task<ActionResult<Cookie>> Login()
    {
        var cookie = await _userOrchestrator.GetNewCookie();
        var user = (await _userService.GetUser(cookie.Value))!;

        await _demoOrchestrator.InitResumes(user.Id);

        return Ok(cookie);
    }

    [HttpDelete("logout")]
    [Authorize(AuthenticationSchemes = "DemoCookie")]
    public async Task<IActionResult> Logout()
    {
        var cookie = Request.Cookies["resume-id"];
        var user = (await _userService.GetUser(cookie!))!;

        await _userService.DeleteUser(user.Id);

        return NoContent();
    }
}