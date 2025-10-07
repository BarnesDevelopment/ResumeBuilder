namespace ResumeAPI.Services;

public interface IAuthClient
{
    Task<bool> AuthenticateJwt(string token);
}

public class AuthClient(HttpClient client): IAuthClient
{
    public async Task<bool> AuthenticateJwt(string token)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/api/auth/validate");
        return response.IsSuccessStatusCode;
    }
}