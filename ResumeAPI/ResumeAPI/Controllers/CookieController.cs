using System.Net;
using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Helpers;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace ResumeAPI.Controllers;

[ApiController]
[Route("resume/cookie")]
public class CookieController : ControllerBase
{
    private readonly ILogger<CookieController> _logger;
    private readonly IUserOrchestrator _userOrchestrator;
    private readonly IUserService _userService;
    private readonly IAnonymousUserValidator _validator;

    public CookieController(
        IAnonymousUserValidator anonymousUserValidator,
        IUserOrchestrator userOrchestrator,
        IUserService userService,
        ILogger<CookieController> logger
    )
    {
        _validator = anonymousUserValidator;
        _userOrchestrator = userOrchestrator;
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(Cookie))]
    public async Task<IActionResult> GetCookie() => Ok(await _userOrchestrator.GetCookie());

    [HttpPost("verify")]
    [ProducesResponseType(200)]
    [ProducesResponseType(200, Type = typeof(Cookie))]
    [ProducesResponseType(204)]
    public async Task<IActionResult> VerifyCookie()
    {
        var valid = await _validator.ValidateCookie(Request);
        switch (valid)
        {
            case CookieValidationResult.Valid: return Ok();
            case CookieValidationResult.Refresh: return Ok(await _userService.GetCookie(Request.Cookies["resume-id"]!));
            case CookieValidationResult.Invalid:
                _logger.LogWarning("Invalid cookie: {Cookie}", Request.Cookies["resume-id"]);
                return NoContent();
            case CookieValidationResult.Expired:
                _logger.LogWarning("Expired cookie {Cookie}", Request.Cookies["resume-id"]);
                return NoContent();
            default: return NoContent();
        }
    }
}