using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/demo")]
public class DemoController(
    IUserService userService,
    IDemoOrchestrator demoOrchestrator,
    ILogger<DemoController> logger
)
    : ControllerBase
{
    [HttpPut("login")]
    public async Task<ActionResult<string>> Login()
    {
        var user = await userService.CreateAnonymousUser();

        await demoOrchestrator.InitResumes(user.id);

        return Ok(user.jwt);
    }

    [HttpDelete("logout")]
    [Authorize(AuthenticationSchemes = Constants.DemoCookieAuth)]
    public async Task<IActionResult> Logout()
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

        if (id == null)
        {
            logger.LogWarning("User not found");
            return Unauthorized();
        }

        var userId = Guid.Parse(id);
        await demoOrchestrator.DeleteUser(userId);
        await userService.DeleteAnonymousUser(userId);

        return NoContent();
    }
}