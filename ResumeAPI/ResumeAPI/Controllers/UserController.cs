using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("users/")]
public class UserController : ControllerBase
{
    private readonly ILogger<ResumeController> _logger;
    private readonly IMySqlContext _db;
    
    public UserController(ILogger<ResumeController> logger, IMySqlContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _db.GetUsers());
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserViewModel userInput)
    {
        var user = new User
        {
            Username = userInput.Username,
            FirstName = userInput.FirstName,
            LastName = userInput.LastName,
            Email = userInput.Email,
            Id = Guid.NewGuid(),
            Salt = PasswordHasher.GenerateSalt(128/8)
        };

        return Ok(await _db.CreateUser(user));
    }
}