using System.Security.Claims;
using ResumeAPI.Database;

namespace ResumeAPI.Helpers;

public interface IUserValidator
{
    Task<UserValidationResult> ValidateUser(Guid userId);
    Task<UserValidationResult> ValidateUser(HttpContext context);
    Task<UserValidationResult> ValidateResource(Guid userId, Guid resourceId);
    Task<UserValidationResult> Validate(HttpContext context, Guid resourceId);
    Guid GetUserId(HttpContext context);
}

public class UserValidator(IUserData userDb, IResumeTree resumeDb) : IUserValidator
{//TODO: Refactor into fluent validation
    private const string UserIdClaim = "userId";

    public async Task<UserValidationResult> ValidateUser(Guid userId)
    {
        // TODO: Rely on fusionauth to validate user existence
        var user = await userDb.GetUser(userId);
        return user != null ? UserValidationResult.Valid : UserValidationResult.Invalid;
    }

    public async Task<UserValidationResult> ValidateUser(HttpContext context)
    {
        if (!Guid.TryParse((context.User.Identity as ClaimsIdentity)!.FindFirst(UserIdClaim)!.Value, out var userId))
            return UserValidationResult.Invalid;
        return await ValidateUser(userId);
    }

    public async Task<UserValidationResult> ValidateResource(Guid userId, Guid resourceId)
    {
        var resource = await resumeDb.GetNode(resourceId);
        if (resource == null) return UserValidationResult.NotFound;
        return resource.UserId == userId ? UserValidationResult.Valid : UserValidationResult.Invalid;
    }

    public async Task<UserValidationResult> Validate(HttpContext context, Guid resourceId)
    {
        if (!Guid.TryParse((context.User.Identity as ClaimsIdentity)!.FindFirst(UserIdClaim)!.Value, out var userId))
            return UserValidationResult.Invalid;
        var user = await ValidateUser(userId);

        if (user != UserValidationResult.Valid) return user;
        return await ValidateResource(userId, resourceId);
    }

    public Guid GetUserId(HttpContext context) =>
        Guid.Parse((context.User.Identity as ClaimsIdentity)!.FindFirst(UserIdClaim)!.Value);
}

public enum UserValidationResult
{
    Tilt,
    Valid,
    Invalid,
    NotFound
}