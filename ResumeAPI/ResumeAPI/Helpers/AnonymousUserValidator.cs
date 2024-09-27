using ResumeAPI.Database;

namespace ResumeAPI.Helpers;

public interface IAnonymousUserValidator
{
    Task<CookieValidationResult> ValidateCookie(HttpRequest request);
}

public class AnonymousUserValidator : IAnonymousUserValidator
{
    private readonly IUserData _userDb;

    public AnonymousUserValidator(IUserData userDb)
    {
        _userDb = userDb;
    }

    public async Task<CookieValidationResult> ValidateCookie(HttpRequest request)
    {
        var cookie = request.Cookies["resume-id"];
        if (cookie == null) return CookieValidationResult.Invalid;
        var user = await _userDb.GetUser(cookie);
        if (user == null) return CookieValidationResult.Invalid;
        if (user.CookieExpiration < DateTime.Now) return CookieValidationResult.Expired;
        if (user.CookieExpiration >= DateTime.Now.AddDays(2)) return CookieValidationResult.Valid;
        await _userDb.UpdateCookieExpiration(user.Id);
        return CookieValidationResult.Refresh;
    }
}

public enum CookieValidationResult
{
    Valid,
    Invalid,
    Refresh,
    Expired
}