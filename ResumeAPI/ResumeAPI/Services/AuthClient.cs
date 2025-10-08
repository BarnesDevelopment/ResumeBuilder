using System.Net;
using io.fusionauth;
using io.fusionauth.domain.api;
using io.fusionauth.domain.search;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IAuthClient
{
    Task<Guid?> CreateAnonymousUser();
    Task<bool> AuthenticateJwt(string token);
    Task<string> VendJwtFromId(Guid id);
}

public class AuthClient : IAuthClient
{
    private readonly FusionAuthClient _authClient;
    private readonly HttpClient _client;

    public AuthClient(HttpClient client, AppSettings appSettings)
    {
        _client = client;
        var authClient = new FusionAuthClient(appSettings.FusionAuth.UserCreationApiKey, appSettings.Jwt.Authority);

        var search = new TenantSearchCriteria()
        {
            name = "Resume Builder",
            numberOfResults = 1
        };
        var tenantSearchRequest = new TenantSearchRequest
        {
            search = search
        };
        var response = authClient.SearchTenantsAsync(tenantSearchRequest).Result;
        var tenantId = response?.successResponse?.tenants[0]?.id;
        if (tenantId == null)
            throw new InvalidOperationException("TenantId cannot be null.");
        
        _authClient = new(appSettings.FusionAuth.UserCreationApiKey, appSettings.Jwt.Authority,tenantId.ToString());
    }
    
    public async Task<bool> AuthenticateJwt(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/jwt/validate");
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
                email = $"demo_{Guid.NewGuid()}@example.com",
                fullName = "Demo User",
                firstName = "Demo",
                lastName = "User",
                // expiry = DateTimeOffset.Now.AddDays(7),
                expiry = DateTime.Now.AddMinutes(10),
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
        const int ttl = 60 * 10; // 10 minutes
        // const int ttl = 60 * 60 * 24 * 7; // 7 days
        var response = await _authClient.VendJWTAsync(new()
        {
            timeToLiveInSeconds = ttl,
            claims = new Dictionary<string, object>
            {
                { "userId", id.ToString() }
            }
        });

        return response.successResponse.token;
    }
}