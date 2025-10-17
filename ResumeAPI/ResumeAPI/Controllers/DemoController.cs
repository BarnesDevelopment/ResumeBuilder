using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Orchestrator;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/demo")]
public class DemoController(
    IDemoOrchestrator demoOrchestrator,
    ILogger<DemoController> logger
)
    : ControllerBase
{
    [HttpPut("login")]
    public async Task<ActionResult<string>> Login() => Ok(await demoOrchestrator.CreateUser());

    [HttpDelete("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        var isAnonymous = User.Claims.FirstOrDefault(c => c.Type == "isAnonymous")?.Value;

        if (id == null)
        {
            logger.LogWarning("User not found");
            return Unauthorized();
        }

        if (isAnonymous != "true")
        {
            logger.LogWarning("User is not anonymous");
            return Forbid();
        }

        var userId = Guid.Parse(id);
        await demoOrchestrator.DeleteUser(userId);

        return NoContent();
    }
}