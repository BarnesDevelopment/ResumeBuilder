using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace UnitTests.Helpers;

public class UserValidatorTests
{
    private readonly Mock<IAuthClient> _authClient;
    private readonly HttpContext _httpContext = new DefaultHttpContext();
    private readonly Mock<IResumeTree> _resumeDb;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly UserValidator _validator;

    public UserValidatorTests()
    {
        _resumeDb = new Mock<IResumeTree>();
        _authClient = new Mock<IAuthClient>();
        _validator = new UserValidator(_resumeDb.Object, _authClient.Object);
        _httpContext.User
            = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("userId", _userId.ToString()) }));
    }

    #region GetUserId(HttpContext context)

    [Fact]
    public void GetUserId()
    {
        var result = _validator.GetUserId(_httpContext);

        result.Should().Be(_userId);
    }

    [Fact]
    public void GetUserId_NoClaim_ThrowsInvalidOperationException()
    {
        _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        
        Action act = () => _ = _validator.GetUserId(_httpContext);
        
        act.Should().Throw<InvalidOperationException>().WithMessage("User ID claim not found");
    }
    #endregion

    #region ValidateUser(HttpContext context)

    [Fact]
    public async Task ValidateUser_ValidUser()
    {
        _httpContext.Request.Headers["Authorization"] = "Bearer valid_token";

        _authClient.Setup(x => x.AuthenticateJwt("valid_token")).ReturnsAsync(true);

        var result = await _validator.ValidateUser(_httpContext);

        result.Should().Be(UserValidationResult.Valid);
    }

    [Fact]
    public async Task ValidateUser_InvalidUser()
    {
        _httpContext.Request.Headers["Authorization"] = "Bearer invalid_token";

        _authClient.Setup(x => x.AuthenticateJwt("invalid_token")).ReturnsAsync(false);

        var result = await _validator.ValidateUser(_httpContext);

        result.Should().Be(UserValidationResult.Invalid);
    }

    #endregion

    #region ValidateResource(Guid userId, Guid resourceId)

    [Fact]
    public async Task ValidateResource_ValidResource()
    {
        var userId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        _resumeDb.Setup(x => x.GetNode(resourceId)).ReturnsAsync(new ResumeTreeNode { UserId = userId });

        var result = await _validator.ValidateResource(userId, resourceId);

        result.Should().Be(UserValidationResult.Valid);
    }

    [Fact]
    public async Task ValidateResource_InvalidResource()
    {
        var userId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        _resumeDb.Setup(x => x.GetNode(resourceId)).ReturnsAsync(new ResumeTreeNode { UserId = Guid.NewGuid() });

        var result = await _validator.ValidateResource(userId, resourceId);

        result.Should().Be(UserValidationResult.Invalid);
    }

    [Fact]
    public async Task ValidateResource_NullResource()
    {
        var userId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        _resumeDb.Setup(x => x.GetNode(resourceId)).ReturnsAsync((ResumeTreeNode)null!);

        var result = await _validator.ValidateResource(userId, resourceId);

        result.Should().Be(UserValidationResult.NotFound);
    }

    [Fact]
    public async Task Validate_UserIdDoesntMatchResource()
    {
        var resourceId = Guid.NewGuid();
        
        _resumeDb.Setup(x => x.GetNode(resourceId)).ReturnsAsync(new ResumeTreeNode { UserId = Guid.NewGuid() });

        var result = await _validator.ValidateResource(_userId, resourceId);

        result.Should().Be(UserValidationResult.Invalid);
    }

    #endregion
}