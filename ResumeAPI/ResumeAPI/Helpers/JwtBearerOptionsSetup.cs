using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace ResumeAPI.Helpers;

public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _options;

    public JwtBearerOptionsSetup(IOptions<AppSettings> appSettings, IAuthClient authClient)
    {
        _options = new JwtOptions
        {
            Issuer = appSettings.Value.Jwt.Authority,
            Audience = authClient.GetApplicationId().ToString(),
            SigningKey = authClient.SigningKey()
        };
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        Console.WriteLine("Configuring JWT Bearer Options with AppId: {0}", _options.Audience);
        Console.WriteLine("Signing Key: {0}", _options.SigningKey);
        Console.WriteLine("Issuer: {0}", _options.Issuer);
        Console.WriteLine("Audience: {0}", _options.Audience);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            RequireSignedTokens = false
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var authClient = ctx.HttpContext.RequestServices.GetRequiredService<IAuthClient>();
                var jwt = ctx.SecurityToken as JwtSecurityToken;
                if (jwt == null)
                {
                    ctx.Fail("Invalid security token.");
                    return;
                }

                var remoteResult = await authClient.AuthenticateJwt(jwt.RawData);
                if (!remoteResult) ctx.Fail("Remote token validation failed.");
            }
        };
    }
}

internal record JwtOptions
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SigningKey { get; init; } = string.Empty;
}