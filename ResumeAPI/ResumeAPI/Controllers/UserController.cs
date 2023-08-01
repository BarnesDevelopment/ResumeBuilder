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

    [HttpGet("")]
    [ProducesResponseType(typeof(UserViewModel[]),200)]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _db.GetUsers());
    }

    [HttpGet("user/{username}")]
    [ProducesResponseType(typeof(UserViewModel),200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser([FromRoute] string username)
    {
        var user = await _db.GetUser(username);
        if(user != null) return Ok(user);
        return NotFound();
    }

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

    [HttpPut("user")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> UpdateUser([FromHeader] string id, [FromHeader] string key)
    {
        var hashedKey = _hasher.HashPassword(key);
        await _db.CreateKey(hashedKey, Guid.Parse(id));
        return Created("Key Created", null);
    }
    
    [HttpPost("user/{id}")]
    [ProducesResponseType(typeof(User),202)]
    public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UserViewModel userInput)
    {
        return Ok(await _db.UpdateUser(Guid.Parse(id),userInput));
    }

    [HttpGet("user")]
    public async Task<IActionResult> VerifyKey([FromHeader] string id, [FromHeader] string key)
    {
        var hash = await _db.RetrieveKey(Guid.Parse(id));
        if (_hasher.VerifyHashedPassword(hash, key) == PasswordVerificationResult.Success) return Ok();
        return NotFound();
    }
    
    [HttpDelete("user/{id}")]
    [ProducesResponseType(202)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateUser([FromRoute] string id)
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
}