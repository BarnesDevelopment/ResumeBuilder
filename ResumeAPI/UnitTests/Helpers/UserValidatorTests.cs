using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace UnitTests.Helpers;

public class UserValidatorTests
{
  private readonly Mock<IUserData> _userDb;
  private readonly Mock<IResumeTree> _resumeDb;
  private readonly UserValidator _validator;

  public UserValidatorTests()
  {
    _userDb = new Mock<IUserData>();
    _resumeDb = new Mock<IResumeTree>();
    _validator = new UserValidator(_userDb.Object, _resumeDb.Object);
  }

  [Fact]
  public async Task ValidateUser_ValidUser()
  {
    var userId = Guid.NewGuid();
    _userDb.Setup(x => x.GetUser(userId)).ReturnsAsync(new User());

    var result = await _validator.ValidateUser(userId);

    result.Should().Be(UserValidationResult.Valid);
  }

  [Fact]
  public async Task ValidateUser_InvalidUser()
  {
    var userId = Guid.NewGuid();
    _userDb.Setup(x => x.GetUser(userId)).ReturnsAsync((User)null!);

    var result = await _validator.ValidateUser(userId);

    result.Should().Be(UserValidationResult.Invalid);
  }

  [Fact]
  public async Task ValidateUser_ValidUserFromContext()
  {
    var userId = Guid.NewGuid();
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("resume-id", userId.ToString()) }))
    };
    _userDb.Setup(x => x.GetUser(userId)).ReturnsAsync(new User());

    var result = await _validator.ValidateUser(context);

    result.Should().Be(UserValidationResult.Valid);
  }

  [Fact]
  public async Task ValidateUser_InvalidUserFromContext()
  {
    var userId = Guid.NewGuid();
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("resume-id", userId.ToString()) }))
    };
    _userDb.Setup(x => x.GetUser(userId)).ReturnsAsync((User)null!);

    var result = await _validator.ValidateUser(context);

    result.Should().Be(UserValidationResult.Invalid);
  }

  [Fact]
  public async Task ValidateUser_InvalidUserIdFromContext()
  {
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("resume-id", "invalid") }))
    };

    var result = await _validator.ValidateUser(context);

    result.Should().Be(UserValidationResult.Invalid);
  }

  [Fact]
  public async Task ValidateResource_ValidResource()
  {
    var userId = Guid.NewGuid();
    var resourceId = Guid.NewGuid();
    _resumeDb.Setup(x => x.GetNode(resourceId)).ReturnsAsync(new ResumeTreeNode() { UserId = userId });

    var result = await _validator.ValidateResource(userId, resourceId);

    result.Should().Be(UserValidationResult.Valid);
  }

  [Fact]
  public async Task ValidateResource_InvalidResource()
  {
    var userId = Guid.NewGuid();
    var resourceId = Guid.NewGuid();
    _resumeDb.Setup(x => x.GetNode(resourceId)).ReturnsAsync(new ResumeTreeNode() { UserId = Guid.NewGuid() });

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
  public async Task Validate_ValidUserAndResource()
  {
    var userId = Guid.NewGuid();
    var resourceId = Guid.NewGuid();
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("resume-id", userId.ToString()) }))
    };
    _userDb.Setup(x => x.GetUser(userId)).ReturnsAsync(new User());
    _resumeDb.Setup(x => x.GetNode(resourceId)).ReturnsAsync(new ResumeTreeNode() { UserId = userId });

    var result = await _validator.Validate(context, resourceId);

    result.Should().Be(UserValidationResult.Valid);
  }

  [Fact]
  public async Task Validate_InvalidUser()
  {
    var userId = Guid.NewGuid();
    var resourceId = Guid.NewGuid();
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("resume-id", userId.ToString()) }))
    };
    _userDb.Setup(x => x.GetUser(userId)).ReturnsAsync((User)null!);

    var result = await _validator.Validate(context, resourceId);

    result.Should().Be(UserValidationResult.Invalid);
  }

  [Fact]
  public async Task Validate_InvalidResource()
  {
    var userId = Guid.NewGuid();
    var resourceId = Guid.NewGuid();
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("resume-id", userId.ToString()) }))
    };
    _userDb.Setup(x => x.GetUser(userId)).ReturnsAsync(new User());
    _resumeDb.Setup(x => x.GetNode(resourceId)).ReturnsAsync((ResumeTreeNode)null!);

    var result = await _validator.Validate(context, resourceId);

    result.Should().Be(UserValidationResult.NotFound);
  }

  [Fact]
  public async Task Validate_UserIdDoesntMatchResource()
  {
    var userId = Guid.NewGuid();
    var resourceId = Guid.NewGuid();
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("resume-id", userId.ToString()) }))
    };
    _userDb.Setup(x => x.GetUser(userId)).ReturnsAsync(new User());
    _resumeDb.Setup(x => x.GetNode(resourceId)).ReturnsAsync(new ResumeTreeNode() { UserId = Guid.NewGuid() });

    var result = await _validator.Validate(context, resourceId);

    result.Should().Be(UserValidationResult.Invalid);
  }

  [Fact]
  public void GetUserId()
  {
    var userId = Guid.NewGuid();
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new("resume-id", userId.ToString()) }))
    };

    var result = _validator.GetUserId(context);

    result.Should().Be(userId);
  }
}
