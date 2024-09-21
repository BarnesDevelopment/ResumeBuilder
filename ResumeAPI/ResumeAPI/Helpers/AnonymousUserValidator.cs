using ResumeAPI.Database;

namespace ResumeAPI.Helpers;

public interface IAnonymousUserValidator
{
  Task<bool> ValidateCookie(HttpRequest request);
}

public class AnonymousUserValidator : IAnonymousUserValidator
{
  private readonly IUserData _userDb;

  public AnonymousUserValidator(IUserData userDb)
  {
    _userDb = userDb;
  }

  public async Task<bool> ValidateCookie(HttpRequest request)
  {
    var cookie = request.Cookies["resume-id"];
    return cookie != null && await _userDb.GetUser(cookie) != null;
  }
}
