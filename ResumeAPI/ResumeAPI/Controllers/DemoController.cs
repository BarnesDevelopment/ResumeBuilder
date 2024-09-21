using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/demo/")]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class DemoController : ControllerBase
{
  [HttpGet]
  public IActionResult Test()
  {
    return Ok("You're in!");
  }
}
