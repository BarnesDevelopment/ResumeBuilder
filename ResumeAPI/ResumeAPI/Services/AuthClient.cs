using io.fusionauth;
using io.fusionauth.domain.api;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IAuthClient
{
    Task<bool> AuthenticateJwt(string token);
}

public class AuthClient(HttpClient client, AppSettings appSettings) : IAuthClient
{
    private readonly FusionAuthClient _authClient = new(appSettings.FusionAuth.UserCreationApiKey, appSettings.Jwt.Authority);

    public async Task<bool> AuthenticateJwt(string token)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/api/jwt/validate");
        return response.IsSuccessStatusCode;
    }

    public async Task<Guid?> CreateAnonymousUser()
    {
        var response = await _authClient.CreateUserAsync(Guid.NewGuid(), new UserRequest
        {
            user = new()
            {
                username=Guid.NewGuid().ToString(),
                password=Guid.NewGuid().ToString(),
                fullName = "Demo User",
                firstName = "Demo",
                lastName = "User",
                expiry = DateTimeOffset.Now.AddDays(7),
                data=new Dictionary<string, object>
                {
                    {"anonymousUser",true},
                }
            }
            
        });

        return response.successResponse.user.id;
    }

    public async Task<string> VendJwtFromId(Guid id)
    {
        const int sevenDaysInSeconds = 60 * 60 * 24 * 7;
        var response = await _authClient.VendJWTAsync(new()
        {
            keyId = id,
            timeToLiveInSeconds = sevenDaysInSeconds,
            claims = new Dictionary<string, object>
            {
                { "userId", id }
            }
        });

        return response.successResponse.token;
    }
}