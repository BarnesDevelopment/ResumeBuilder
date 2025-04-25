using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using ResumeAPI.Database;
using ResumeAPI.Models;

namespace ResumeAPI.DemoCookieAuth;

public class DemoCookieAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUserData _userDb;

    public DemoCookieAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserData userDb
    ) : base(options, logger, encoder, clock)
    {
        _userDb = userDb;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var cookie = Context.Request.Cookies["resume-id"];

        if (cookie == null) return AuthenticateResult.NoResult();

        var user = await _userDb.GetUser(cookie);

        if (user == null) return AuthenticateResult.Fail("Invalid user");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "DemoUser"),
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Role, "User"),
            new(ClaimTypes.AuthenticationMethod, Constants.DemoCookieAuth),
            new("resume-id", user.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, Constants.DemoCookieAuth);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}