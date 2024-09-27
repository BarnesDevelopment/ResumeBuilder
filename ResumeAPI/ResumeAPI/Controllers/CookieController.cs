using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Helpers;
using ResumeAPI.Orchestrator;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/cookie")]
public class CookieController : ControllerBase
{
    private readonly IUserOrchestrator _userOrchestrator;
    private readonly IAnonymousUserValidator _validator;

    public CookieController(IAnonymousUserValidator anonymousUserValidator, IUserOrchestrator userOrchestrator)
    {
        _validator = anonymousUserValidator;
        _userOrchestrator = userOrchestrator;
    }

    // [HttpGet]
    // public async Task<IActionResult> Test()
    // {
    //     if (await _validator.ValidateCookie(Request)) return Unauthorized("You're not in!");
    //     return Ok("You're in!");
    // }

    [HttpGet]
    public async Task<IActionResult> GetCookie() => Ok(await _userOrchestrator.GetCookie());

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyCookie() => throw new NotImplementedException();

    [HttpGet("refresh")]
    public async Task<IActionResult> RefreshCookie() => throw new NotImplementedException();

    [HttpDelete]
    public async Task<IActionResult> DeleteCookie() => throw new NotImplementedException();

    [HttpGet("user")]
    public async Task<IActionResult> GetUser() => throw new NotImplementedException();
}