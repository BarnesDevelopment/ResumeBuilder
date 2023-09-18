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
    /// Get all users
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet("")]
    [ProducesResponseType(typeof(UserViewModel[]),200)]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            return Ok(await _service.GetUsers());
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            return Problem(e.Message);
        }
        
    }
    
    /// <summary>
    /// Gets user by id
    /// Requires Authorization header with cookie
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("user")]
    [ProducesResponseType(typeof(UserViewModel),200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser([FromQuery] string userId)
    {
        try
        {
            var cookie = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(cookie)) return Unauthorized();
            var user = await _service.GetUser(Guid.Parse(userId));
            if (user != null)
            {
                if (!await _service.VerifyCookie(Guid.Parse(userId), Guid.Parse(cookie))) return Unauthorized();
                return Ok(user);
            }
            return NotFound();
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            return Problem(e.Message);
        }
    }

    /// <summary>
    /// Create user
    /// </summary>
    /// <param name="userInput">User object</param>
    /// <param name="key"></param>
    /// <returns></returns>
    [HttpPost("user")]
    [ProducesResponseType(typeof(User),201)]
    public async Task<IActionResult> CreateUser([FromBody] UserInfo userInput, [FromHeader] string key)
    {
        try
        {
            return Ok(await _orchestrator.CreateAccount(new UserViewModel(userInput), key));
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            return Problem(e.Message);
        }
    }
    
    /// <summary>
    /// Update user information
    /// Requires Authorization header with cookie
    /// </summary>
    /// <param name="id">UserId</param>
    /// <param name="userInput">User object</param>
    /// <returns></returns>
    [HttpPost("user/{id}")]
    [ProducesResponseType(typeof(User),202)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UserInfo userInput)
    {
        try
        {
            var cookie = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(cookie)) return Unauthorized();
            if (!await _service.VerifyCookie(Guid.Parse(id), Guid.Parse(cookie))) return Unauthorized();
            return Ok(await _service.UpdateUser(id,new UserViewModel(userInput)));
        }
        catch(Exception e)
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
            if (await _orchestrator.DeleteUser(Guid.Parse(id)))
            {
                return Accepted();
            }
            return NotFound();
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            return Problem(e.Message);
        }
        
    }
    
    #endregion

    #region Key

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="username"></param>
    /// <param name="key"></param>
    /// <returns>Cookie</returns>
    [HttpGet("login")]
    [ProducesResponseType(typeof(Cookie),200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromHeader] string username, [FromHeader] string key)
    {
        try
        {
            var attempt = await _orchestrator.Login(username, key);
            if (attempt.Success)
            {
                return Ok(attempt.Cookie);
            }

            return Unauthorized();
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            return Problem(e.Message);
        }
    }
    
    #endregion
}