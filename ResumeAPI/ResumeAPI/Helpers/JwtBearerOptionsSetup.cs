using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
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
        var signingKeyInfo = authClient.GetSigningKey();
        _options = new JwtOptions
        {
            Issuer = appSettings.Value.Jwt.Authority,
            Audience = authClient.GetApplicationId().ToString(),
            SigningKey = signingKeyInfo.publicKey,
            Kid = signingKeyInfo.kid
        };
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }

    public void Configure(string name, JwtBearerOptions options)
    {
        #if DEBUG
        Console.WriteLine("Configuring JWT Bearer Options with AppId: {0}", _options.Audience);
        Console.WriteLine("Issuer: {0}", _options.Issuer);
        Console.WriteLine("Audience: {0}", _options.Audience);
        Console.WriteLine("Signing Key: {0}", _options.SigningKey);
        Console.WriteLine("Key Id: {0}", _options.Kid);
        #endif

        var pem = _options.SigningKey
            .Replace("-----BEGIN PUBLIC KEY-----", string.Empty)
            .Replace("-----END PUBLIC KEY-----", string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Trim();
        var der = Convert.FromBase64String(pem);
        var ecdsa = ECDsa.Create();
        ecdsa.ImportSubjectPublicKeyInfo(der, out _);
        var key = new ECDsaSecurityKey(ecdsa) { KeyId = _options.Kid };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            RequireSignedTokens = true
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
    public string Kid { get; init; } = string.Empty;
}