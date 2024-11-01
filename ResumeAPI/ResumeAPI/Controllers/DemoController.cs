using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Helpers;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/demo/")]
[Authorize(AuthenticationSchemes = "DemoCookie")]
public class DemoController : ControllerBase
{
    private readonly IDemoOrchestrator _demoOrchestrator;
    private readonly IUserService _userService;
    private readonly IAnonymousUserValidator _validator;

    public DemoController(
        IAnonymousUserValidator anonymousUserValidator,
        IUserService userService,
        IDemoOrchestrator demoOrchestrator
    )
    {
        _validator = anonymousUserValidator;
        _userService = userService;
        _demoOrchestrator = demoOrchestrator;
    }

    [HttpGet]
    public async Task<IActionResult> Test()
    {
        return Ok("You're in!");
    }

    public async Task<IActionResult> InitResumes()
    {
        var cookie = Request.Cookies["resume-id"];
        var user = (await _userService.GetUser(cookie!))!;

        await _demoOrchestrator.InitResumes(user.Id);

        return Ok();
    }
}