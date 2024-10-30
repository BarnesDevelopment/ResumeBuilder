using Microsoft.AspNetCore.Authentication;

namespace ResumeAPI.DemoCookieAuth;

public static class ExtensionMethods
{
    public static AuthenticationBuilder UseDemoCookieAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<AuthenticationSchemeOptions, DemoCookieAuthenticationHandler>("DemoCookie",
            options => { });
    }
}