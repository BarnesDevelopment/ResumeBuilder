using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;
using PasswordVerificationResult = ResumeAPI.Helpers.PasswordVerificationResult;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("users/")]
public class UserController : ControllerBase
{
    private readonly ILogger<ResumeController> _logger;
    private readonly IUserService _service;
    private readonly IUserOrchestrator _orchestrator;
    
    public UserController(ILogger<ResumeController> logger, IUserService service, IUserOrchestrator orchestrator)
    {
        _logger = logger;
        _service = service;
        _orchestrator = orchestrator;
    }

    #region User
    
    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(UserViewModel[]),200)]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _service.GetUsers());
    }
    
    /// <summary>
    /// Gets user by username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [HttpGet("user")]
    [ProducesResponseType(typeof(UserViewModel),200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser([FromQuery] string username)
    {
        var user = await _service.GetUser(username);
        if(user != null) return Ok(user);
        return NotFound();
    }

    /// <summary>
    /// Create user
    /// </summary>
    /// <param name="userInput">User object</param>
    /// <returns></returns>
    [HttpPost("user")]
    [ProducesResponseType(typeof(User),201)]
    public async Task<IActionResult> CreateUser([FromBody] UserViewModel userInput, [FromHeader] string key)
    {
        return Ok(await _orchestrator.CreateAccount(userInput, key));
    }
    
    /// <summary>
    /// Update user information
    /// </summary>
    /// <param name="id">UserId</param>
    /// <param name="userInput">User object</param>
    /// <returns></returns>
    [HttpPost("user/{id}")]
    [ProducesResponseType(typeof(User),202)]
    public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UserViewModel userInput)
    {
        return Ok(await _service.UpdateUser(id,userInput));
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
        if (await _service.DeleteUser(id))
        {
            return Accepted();
        }
        return NotFound();
        
    }
    
    #endregion

    #region Key

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="username"></param>
    /// <param name="key"></param>
    /// <returns>Cookie</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Cookie),200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromHeader] string username, [FromHeader] string key)
    {
        var attempt = await _orchestrator.Login(username, key);
        if (attempt.Success)
        {
            return Ok(attempt.Cookie);
        }

        return Unauthorized();
    }
    
    #endregion
}