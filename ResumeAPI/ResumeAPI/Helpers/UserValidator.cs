using ResumeAPI.Database;

namespace ResumeAPI.Helpers;

public interface IUserValidator
{
  Task<UserValidationResult> ValidateUser(Guid userId);
  Task<UserValidationResult> ValidateResource(Guid userId, Guid resourceId);
}

public class UserValidator : IUserValidator
{
  private readonly IUserData _userDb;
  private readonly IResumeTree _resumeDb;
  
  public UserValidator(IUserData userDb, IResumeTree resumeDb)
  {
    _userDb = userDb;
    _resumeDb = resumeDb;
  }

  public async Task<UserValidationResult> ValidateUser(Guid userId)
  {
    var user = await _userDb.GetUser(userId);
    return user != null ? UserValidationResult.Valid : UserValidationResult.Invalid;
  }
  
  public async Task<UserValidationResult> ValidateResource(Guid userId, Guid resourceId)
  {
    var resource = await _resumeDb.GetNode(resourceId);
    if(resource == null)
    {
      return UserValidationResult.NotFound;
    }
    return resource.UserId == userId ? UserValidationResult.Valid : UserValidationResult.Invalid;
  }
}

public enum UserValidationResult
{
  Valid,
  Invalid,
  NotFound
}
