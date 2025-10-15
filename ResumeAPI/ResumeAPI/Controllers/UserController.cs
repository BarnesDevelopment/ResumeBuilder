using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/users")]
public class UserController(ILogger<UserController> logger, IUserService service, IUserOrchestrator orchestrator)
    : ControllerBase
{
    private readonly IUserService _service = service;

    #region User

    /// <summary>
    /// Create user
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(User), 201)]
    public async Task<IActionResult> CreateUser()
    {
        try
        {
            return Created("", await orchestrator.CreateUser());
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Problem(e.Message);
        }
    }

    /// <summary>
    /// Delete User
    /// </summary>
    /// <param name="id">UserId</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(202)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
    {
        try
        {
            if (await orchestrator.DeleteUser(Guid.Parse(id))) return Accepted();
            return NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Problem(e.Message);
        }
    }

    #endregion
}