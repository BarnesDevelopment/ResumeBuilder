using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Services;
using ResumeAPI.Validators;

namespace UnitTests.Validators;

public class UserCredentialsTests
{
    private readonly Mock<IAuthClient> _authClient;
    private readonly Mock<IResumeTree> _resumeDb;
    private readonly UserCredentials _validator;

    public UserCredentialsTests()
    {
        _resumeDb = new Mock<IResumeTree>();
        _authClient = new Mock<IAuthClient>();
        _validator = new UserCredentials(_resumeDb.Object, _authClient.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task ValidCredentials(string? resourceIdString)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer valid_token";
        Guid? resourceId = Guid.TryParse(resourceIdString, out var guid) ? guid : null;
        httpContext.Request.Headers["Authorization"] = "Bearer valid_token";
        httpContext.User
            = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new("userId", resourceId.ToString() ?? Guid.NewGuid().ToString())
            }));

        _authClient.Setup(a => a.AuthenticateJwt("valid_token")).ReturnsAsync(true);
        if (resourceId.HasValue)
        {
            _resumeDb.Setup(r => r.GetNode(resourceId.Value))
                .ReturnsAsync(new ResumeTreeNode { UserId = resourceId.Value });
        }

        var actual = await _validator.ValidateAsync((httpContext, resourceId));

        actual.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MissingAuthorizationHeader()
    {
        var httpContext = new DefaultHttpContext();
        var resourceId = Guid.NewGuid();

        var actual = await _validator.ValidateAsync((httpContext, resourceId));

        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().ContainSingle(e => e.ErrorMessage == "Missing Authorization header. 401");
    }

    [Fact]
    public async Task EmptyAuthorizationHeader()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "";
        var resourceId = Guid.NewGuid();

        var actual = await _validator.ValidateAsync((httpContext, resourceId));

        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().ContainSingle(e => e.ErrorMessage == "Authorization header cannot be empty. 401");
    }

    [Fact]
    public async Task AuthorizationHeaderNotStartingWithBearer()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "InvalidHeaderValue";
        var resourceId = Guid.NewGuid();

        var actual = await _validator.ValidateAsync((httpContext, resourceId));

        actual.IsValid.Should().BeFalse();
        actual.Errors.Should()
            .ContainSingle(e => e.ErrorMessage == "Authorization header must start with 'Bearer '. 401");
    }

    [Fact]
    public async Task InvalidToken()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer invalid_token";
        var resourceId = Guid.NewGuid();

        _authClient.Setup(a => a.AuthenticateJwt("invalid_token")).ReturnsAsync(false);

        var actual = await _validator.ValidateAsync((httpContext, resourceId));

        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().ContainSingle(e => e.ErrorMessage == "Invalid Authorization header. 401");
    }

    // resource id is provided but not found in database
    // resource id is provided but does not belong to user

    [Fact]
    public async Task ResourceNotFound()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer valid_token";
        var resourceId = Guid.NewGuid();

        _authClient.Setup(a => a.AuthenticateJwt("valid_token")).ReturnsAsync(true);
        _resumeDb.Setup(r => r.GetNode(resourceId)).ReturnsAsync((ResumeTreeNode?)null);

        var actual = await _validator.ValidateAsync((httpContext, resourceId));

        actual.IsValid.Should().BeFalse();
        actual.Errors.Should()
            .ContainSingle(e => e.ErrorMessage == "The specified resource does not exist. 404");
    }

    [Fact]
    public async Task ResourceDoesNotBelongToUser()
    {
        var httpContext = new DefaultHttpContext();
        var resourceId = Guid.NewGuid();
        httpContext.Request.Headers["Authorization"] = "Bearer valid_token";
        httpContext.User
            = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("userId", resourceId.ToString()) }));

        _authClient.Setup(a => a.AuthenticateJwt("valid_token")).ReturnsAsync(true);
        _resumeDb.Setup(r => r.GetNode(resourceId)).ReturnsAsync(new ResumeTreeNode { UserId = Guid.NewGuid() });

        var actual = await _validator.ValidateAsync((httpContext, resourceId));

        actual.IsValid.Should().BeFalse();
        actual.Errors.Should()
            .ContainSingle(e => e.ErrorMessage == "User does not have access to the specified resource. 403");
    }
}