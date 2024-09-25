using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Helpers;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/demo/")]
public class DemoController : ControllerBase
{
    private readonly IAnonymousUserValidator _validator;

    public DemoController(IAnonymousUserValidator anonymousUserValidator)
    {
        _validator = anonymousUserValidator;
    }

    [HttpGet]
    public async Task<IActionResult> Test()
    {
        if (await _validator.ValidateCookie(Request)) return Unauthorized("You're not in!");
        return Ok("You're in!");
    }
}