using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using PasswordVerificationResult = ResumeAPI.Helpers.PasswordVerificationResult;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("users/")]
public class UserController : ControllerBase
{
    private readonly ILogger<ResumeController> _logger;
    private readonly IMySqlContext _db;
    private readonly PasswordHasher _hasher;
    
    public UserController(ILogger<ResumeController> logger, IMySqlContext db)
    {
        _logger = logger;
        _db = db;
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
        return Ok(await _db.GetUsers());
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
        var user = await _db.GetUser(username);
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
        var user = new User
        {
            Username = userInput.Username,
            FirstName = userInput.FirstName,
            LastName = userInput.LastName,
            Email = userInput.Email,
            Id = Guid.NewGuid().ToString(),
        };

        return Ok(await _db.CreateUser(user));
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
        return Ok(await _db.UpdateUser(Guid.Parse(id),userInput));
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
        var success = await _db.DeleteUser(Guid.Parse(id));

        if (success)
        {
            return Accepted();
        }
        else
        {
            return NotFound();
        }
        
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
    public async Task<IActionResult> VerifyKey([FromHeader] string id, [FromHeader] string key)
    {
        var hash = await _db.RetrieveKey(Guid.Parse(id));
        if (hash != null)
        {
            if (_hasher.VerifyHashedPassword(hash, key) == PasswordVerificationResult.Success) return Ok("Accepted");
            return Ok("Denied");
        }
        return NotFound("No key found for user.");
    }
    
    #endregion
}