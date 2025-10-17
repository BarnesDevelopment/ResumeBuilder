using ResumeAPI.Database;

namespace ResumeAPI.Services;

public interface IUserService
{
    Task<(string jwt, Guid id)> CreateAnonymousUser();
    Task<bool> DeleteAnonymousUser(Guid id);
}

public class UserService(IUserData db, IAuthClient authClient) : IUserService
{
    public async Task<(string jwt, Guid id)> CreateAnonymousUser()
    {
        var id = await authClient.CreateAnonymousUser();
        if (id == null) throw new Exception("Failed to create anonymous user");
        return (await authClient.VendJwtFromId(id.Value), id.Value);
    }

    public async Task<bool> DeleteAnonymousUser(Guid id) => await authClient.DeleteUser(id);
}