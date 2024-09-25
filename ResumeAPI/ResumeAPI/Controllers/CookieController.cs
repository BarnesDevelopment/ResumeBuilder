using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Helpers;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/cookie/")]
public class CookieController : ControllerBase
{
    private readonly IAnonymousUserValidator _validator;

    public CookieController(IAnonymousUserValidator anonymousUserValidator)
    {
        _validator = anonymousUserValidator;
    }

    [HttpGet]
    public async Task<IActionResult> Test()
    {
        if (await _validator.ValidateCookie(Request)) return Unauthorized("You're not in!");
        return Ok("You're in!");
    }

    [HttpGet("cookie")]
    public async Task<IActionResult> GetCookie() => throw new NotImplementedException();

    [HttpPost("cookie/verify")]
    public async Task<IActionResult> VerifyCookie() => throw new NotImplementedException();

    [HttpGet("cookie/refresh")]
    public async Task<IActionResult> RefreshCookie() => throw new NotImplementedException();

    [HttpDelete("cookie")]
    public async Task<IActionResult> DeleteCookie() => throw new NotImplementedException();

    [HttpGet("cookie/user")]
    public async Task<IActionResult> GetUser() => throw new NotImplementedException();
}