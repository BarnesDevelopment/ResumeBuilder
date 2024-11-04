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

public class UserValidator : IUserValidator
{
    private readonly IResumeTree _resumeDb;
    private readonly IUserData _userDb;

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

    public async Task<UserValidationResult> ValidateUser(HttpContext context)
    {
        if (!Guid.TryParse((context.User.Identity as ClaimsIdentity)!.FindFirst("resume-id")!.Value, out var userId))
            return UserValidationResult.Invalid;
        return await ValidateUser(userId);
    }

    public async Task<UserValidationResult> ValidateResource(Guid userId, Guid resourceId)
    {
        var resource = await _resumeDb.GetNode(resourceId);
        if (resource == null) return UserValidationResult.NotFound;
        return resource.UserId == userId ? UserValidationResult.Valid : UserValidationResult.Invalid;
    }

    public async Task<UserValidationResult> Validate(HttpContext context, Guid resourceId)
    {
        if (!Guid.TryParse((context.User.Identity as ClaimsIdentity)!.FindFirst("resume-id")!.Value, out var userId))
            return UserValidationResult.Invalid;
        var user = await ValidateUser(userId);

        if (user != UserValidationResult.Valid) return user;
        return await ValidateResource(userId, resourceId);
    }

    public Guid GetUserId(HttpContext context) =>
        Guid.Parse((context.User.Identity as ClaimsIdentity)!.FindFirst("resume-id")!.Value);
}

public enum UserValidationResult
{
    Tilt,
    Valid,
    Invalid,
    NotFound
}