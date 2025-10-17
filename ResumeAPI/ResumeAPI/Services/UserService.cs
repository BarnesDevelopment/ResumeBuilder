namespace ResumeAPI.Services;

public interface IUserService
{
    Task<(string jwt, Guid id)> CreateAnonymousUser();
    Task<bool> DeleteAnonymousUser(Guid id);
}

public class UserService(IAuthClient authClient) : IUserService
{
    public async Task<(string jwt, Guid id)> CreateAnonymousUser()
    {
        var id = await authClient.CreateAnonymousUser();
        return id == null
            ? throw new Exception("Failed to create anonymous user")
            : (await authClient.VendJwtFromId(id.Value), id.Value);
    }

    public async Task<bool> DeleteAnonymousUser(Guid id) => await authClient.DeleteUser(id);
}