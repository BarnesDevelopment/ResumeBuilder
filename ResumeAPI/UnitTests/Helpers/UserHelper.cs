using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ResumeAPI.Helpers;

namespace UnitTests.Helpers;

public class UserHelper
{
    private readonly HttpContext _httpContext = new DefaultHttpContext();
    private readonly Guid _userId = Guid.NewGuid();

    public UserHelper()
    {
        _httpContext.User
            = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("userId", _userId.ToString()) }));
    }

    #region GetUserId(HttpContext context)

    [Fact]
    public void GetUserId()
    {
        var result = _httpContext.GetUserId();

        result.Should().Be(_userId);
    }

    [Fact]
    public void GetUserId_NoClaim_ThrowsInvalidOperationException()
    {
        _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        Action act = () => _ = _httpContext.GetUserId();

        act.Should().Throw<InvalidOperationException>().WithMessage("User ID claim not found");
    }

    #endregion
}