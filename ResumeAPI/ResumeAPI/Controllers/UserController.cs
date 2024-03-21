using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;
using Cookie = ResumeAPI.Models.Cookie;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("users/")]
public class UserController : ControllerBase
{
  private readonly ILogger<UserController> _logger;
  private readonly IUserService _service;
  private readonly IUserOrchestrator _orchestrator;

  public UserController(ILogger<UserController> logger, IUserService service, IUserOrchestrator orchestrator)
  {
    _logger = logger;
    _service = service;
    _orchestrator = orchestrator;
  }

  #region User

  /// <summary>
  /// Create user
  /// </summary>
  /// <param name="userInput">User object</param>
  /// <param name="key"></param>
  /// <returns></returns>
  [HttpPost("user")]
  [ProducesResponseType(typeof(User), 201)]
  public async Task<IActionResult> CreateUser([FromBody] UserInfo userInput, [FromHeader] string key)
  {
    try
    {
      return Ok(await _orchestrator.CreateAccount(new UserViewModel(userInput), key));
    }
    catch (Exception e)
    {
      _logger.LogError(e.Message);
      return Problem(e.Message);
    }
  }

  /// <summary>
  /// Delete User
  /// </summary>
  /// <param name="id">UserId</param>
  /// <returns></returns>
  [HttpDelete("user/{id}")]
  [ProducesResponseType(202)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> DeleteUser([FromRoute] string id)
  {
    try
    {
      if (await _orchestrator.DeleteUser(Guid.Parse(id))) return Accepted();
      return NotFound();
    }
    catch (Exception e)
    {
      _logger.LogError(e.Message);
      return Problem(e.Message);
    }
  }

  #endregion
}
