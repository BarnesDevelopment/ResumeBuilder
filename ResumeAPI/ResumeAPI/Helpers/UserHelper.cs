using System.Security.Claims;

namespace ResumeAPI.Helpers;

public static class UserHelper
{
    private const string UserIdClaim = "userId";

    public static Guid GetUserId(this HttpContext context) =>
        !Guid.TryParse((context.User.Identity as ClaimsIdentity)?.FindFirst(UserIdClaim)?.Value, out var userId)
            ? throw new InvalidOperationException("User ID claim not found")
            : userId;
}