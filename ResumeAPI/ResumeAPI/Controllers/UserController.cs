using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;
using PasswordVerificationResult = ResumeAPI.Helpers.PasswordVerificationResult;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("users/")]
public class UserController : ControllerBase
{
    private readonly ILogger<ResumeController> _logger;
    private readonly IMySqlContext _db;
    private readonly PasswordHasher _hasher;
    private readonly IUserService _service;
    
    public UserController(ILogger<ResumeController> logger, IMySqlContext db, IUserService service)
    {
        _logger = logger;
        _db = db;
        _service = service;
        _hasher = new PasswordHasher();
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
    public async Task<IActionResult> CreateUser([FromBody] UserViewModel userInput)
    {
        return Ok(await _service.CreateUser(userInput));
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
    /// Create Pass Key
    /// </summary>
    /// <param name="id">UserId</param>
    /// <param name="key">PassKey</param>
    /// <returns></returns>
    [HttpPut("user/key")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> CreateKey([FromHeader] string id, [FromHeader] string key)
    {
        var hashedKey = _hasher.HashPassword(key);
        await _db.CreateKey(hashedKey, Guid.Parse(id));
        return Created("Key Created", null);
    }
    
    /// <summary>
    /// Verify Pass Key
    /// </summary>
    /// <param name="id">UserId</param>
    /// <param name="key">PassKey</param>
    /// <returns></returns>
    [HttpGet("user/key")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> VerifyKey([FromHeader] string id, [FromHeader] string key)
    {
        var hash = await _db.RetrieveKey(Guid.Parse(id));
        if (hash != null)
        {
            if (_hasher.VerifyHashedPassword(hash, key) == PasswordVerificationResult.Success) return Ok();
            return Unauthorized();
        }
        return NotFound("No key found for user.");
    }
    
    #endregion
}