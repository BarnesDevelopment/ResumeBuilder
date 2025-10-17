using System.Security.Claims;
using ResumeAPI.Database;
using ResumeAPI.Services;

namespace ResumeAPI.Helpers;

public interface IUserValidator
{
    Task<UserValidationResult> Validate(HttpContext context, Guid resourceId);
    Task<UserValidationResult> ValidateUser(HttpContext context);
    Task<UserValidationResult> ValidateResource(Guid userId, Guid resourceId);
    Guid GetUserId(HttpContext context);
}

public class UserValidator(IResumeTree resumeDb, IAuthClient authClient) : IUserValidator
{ //TODO: Refactor into fluent validation
    private const string UserIdClaim = "userId";

    public async Task<UserValidationResult> Validate(HttpContext context, Guid resourceId)
    {
        var userValidationResult = await ValidateUser(context);
        if (userValidationResult != UserValidationResult.Valid)
            return userValidationResult;
        var userId = GetUserId(context);
        return await ValidateResource(userId, resourceId);
    }

    public async Task<UserValidationResult> ValidateUser(HttpContext context)
    {
        context.Request.Headers.TryGetValue("Authorization", out var authHeader);
        if (string.IsNullOrEmpty(authHeader)) return UserValidationResult.Invalid;

        if (authHeader.ToString().StartsWith("Bearer "))
        {
            var token = authHeader.ToString().Substring("Bearer ".Length).Trim();
            var response = await authClient.AuthenticateJwt(token);
            if (!response) return UserValidationResult.Invalid;
            return UserValidationResult.Valid;
        }

        return UserValidationResult.Invalid;
    }

    public async Task<UserValidationResult> ValidateResource(Guid userId, Guid resourceId)
    {
        var resource = await resumeDb.GetNode(resourceId);
        if (resource == null) return UserValidationResult.NotFound;
        return resource.UserId == userId ? UserValidationResult.Valid : UserValidationResult.Invalid;
    }

    public Guid GetUserId(HttpContext context) =>
        !Guid.TryParse((context.User.Identity as ClaimsIdentity)?.FindFirst(UserIdClaim)?.Value, out var userId)
            ? throw new InvalidOperationException("User ID claim not found")
            : userId;
}

public enum UserValidationResult
{
    Tilt,
    Valid,
    Invalid,
    NotFound
}