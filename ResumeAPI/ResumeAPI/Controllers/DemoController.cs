using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Models;
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

        Response.Cookies.Append("resume-id",
            cookie.Value,
            new CookieOptions
            {
                HttpOnly = false, Secure = false, SameSite = SameSiteMode.Strict, MaxAge = TimeSpan.FromDays(1)
            });

        return Ok(cookie);
    }

    [HttpDelete("logout")]
    [Authorize(AuthenticationSchemes = Constants.DemoCookieAuth)]
    public async Task<IActionResult> Logout()
    {
        var cookie = Request.Cookies["resume-id"];
        var user = await _userService.GetUser(cookie!);

        if (user == null)
        {
            _logger.LogWarning("User not found for cookie {Cookie}", cookie);
            return NotFound();
        }

        await _demoOrchestrator.DeleteUser(user.Id);

        return NoContent();
    }
}