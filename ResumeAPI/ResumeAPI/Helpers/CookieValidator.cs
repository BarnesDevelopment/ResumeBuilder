using ResumeAPI.Services;

namespace ResumeAPI.Helpers;

public interface ICookieValidator
{
    Task<Guid?> Validate(string cookie);
}

public class CookieValidator : ICookieValidator
{
    private readonly IUserService _service;
    
    public CookieValidator(IUserService service)
    {
        _service = service;
    }
    
    public async Task<Guid?> Validate(string cookie)
    {
        if (string.IsNullOrEmpty(cookie)) return null;
        var user = await _service.GetUserByCookie(Guid.Parse(cookie));
        if (user == Guid.Empty) return null;

        if (!await _service.VerifyCookie(user, Guid.Parse(cookie))) return null;
        return user;
    }
}