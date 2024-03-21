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
}
