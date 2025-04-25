using Microsoft.AspNetCore.Authentication;
using ResumeAPI.Models;

namespace ResumeAPI.DemoCookieAuth;

public static class ExtensionMethods
{
    public static AuthenticationBuilder UseDemoCookieAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<AuthenticationSchemeOptions, DemoCookieAuthenticationHandler>(Constants.DemoCookieAuth,
            options => { });
    }
}