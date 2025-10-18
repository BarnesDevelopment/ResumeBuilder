using System.Net.Http.Headers;
using io.fusionauth;
using io.fusionauth.domain.api;
using io.fusionauth.domain.api.jwt;
using io.fusionauth.domain.search;
using ResumeAPI.Models;
using User = io.fusionauth.domain.User;

namespace ResumeAPI.Services;

public interface IAuthClient
{
    Task<Guid?> CreateAnonymousUser();
    Task<bool> AuthenticateJwt(string token);
    Task<string> VendJwtFromId(Guid id);
    Task<bool> DeleteUser(Guid id);
    Guid GetApplicationId();
    (string publicKey, string kid) GetSigningKey();
}

public class AuthClient : IAuthClient
{
    private readonly Guid _appId;
    private readonly FusionAuthClient _authClient;
    private readonly HttpClient _client;
    private readonly string _issuer;

    public AuthClient(HttpClient client, AppSettings appSettings)
    {
        _client = client;
        var authClient = new FusionAuthClient(appSettings.FusionAuth.UserCreationApiKey,
            $"{appSettings.Jwt.Authority}");

        var search = new TenantSearchCriteria { name = "Resume Builder", numberOfResults = 1 };
        var tenantSearchRequest = new TenantSearchRequest { search = search };
        var response = authClient.SearchTenantsAsync(tenantSearchRequest).Result;
        var tenantId = response?.successResponse?.tenants[0]?.id;
        if (tenantId == null) throw new InvalidOperationException("TenantId cannot be null.");

        var apps = authClient.SearchApplicationsAsync(new ApplicationSearchRequest
            {
                search = new ApplicationSearchCriteria
                {
                    tenantId = tenantId, name = "Resume Builder", numberOfResults = 1
                }
            })
            .Result;
        if (apps.successResponse == null || apps.successResponse.applications.Count == 0)
            throw new InvalidOperationException("Application not found.");

        _appId = apps!.successResponse!.applications[0]!.id!.Value;

        _issuer = response!.successResponse!.tenants[0]!.issuer;
        _authClient = new FusionAuthClient(appSettings.FusionAuth.UserCreationApiKey,
            appSettings.Jwt.Authority,
            tenantId.ToString());
    }

    public (string publicKey, string kid) GetSigningKey()
    {
        var key = _authClient.RetrieveKeysAsync()
            .Result.successResponse.keys.First(x => x.name == "Resume Builder Signing Key");
        return (key.publicKey, key.kid);
    }

    public async Task<bool> AuthenticateJwt(string token)
    {
        _client.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/jwt/validate");
        return response.IsSuccessStatusCode;
    }

    public Guid GetApplicationId() => _appId;

    public async Task<Guid?> CreateAnonymousUser()
    {
        var password = Guid.NewGuid().ToString();
        var username = Guid.NewGuid().ToString();
        var response = await _authClient.CreateUserAsync(Guid.NewGuid(),
            new UserRequest
            {
                user = new User
                {
                    username = username,
                    password = password,
                    email = $"demo_{Guid.NewGuid()}@example.com",
                    fullName = "Demo User",
                    firstName = "Demo",
                    lastName = "User",
                    // expiry = DateTimeOffset.Now.AddDays(7),
                    expiry = DateTime.Now.AddMinutes(10),
                    data = new Dictionary<string, object> { { "anonymousUser", true } }
                }
            });

        return response.successResponse.user.id;
    }

    public async Task<string> VendJwtFromId(Guid id)
    {
        const int ttl = 60 * 10; // 10 minutes
        // const int ttl = 60 * 60 * 24 * 7; // 7 days
        var response = await _authClient.VendJWTAsync(new JWTVendRequest
        {
            timeToLiveInSeconds = ttl,
            claims = new Dictionary<string, object>
            {
                { "userId", id.ToString() },
                { "anonymousUser", true },
                { "iss", _issuer },
                { "aud", _appId.ToString() }
            }
        });

        return response.successResponse.token;
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        var response = await _authClient.DeleteUserAsync(id);

        return response.WasSuccessful();
    }
}