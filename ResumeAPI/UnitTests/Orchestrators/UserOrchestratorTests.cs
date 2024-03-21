using Moq;
using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace UnitTests.Orchestrators;

public class UserOrchestratorTests
{
  private readonly Mock<IUserService> _service;
  private readonly Mock<IUserData> _db;
  private readonly IUserOrchestrator _orchestrator;

  public UserOrchestratorTests()
  {
    _service = new Mock<IUserService>();
    _db = new Mock<IUserData>();
    _orchestrator = new UserOrchestrator(_service.Object, _db.Object);
  }

  [Fact]
  public async Task CreateUser_WhenCalled_ShouldReturnUserId()
  {
    var userId = Guid.NewGuid();
    _service.Setup(x => x.CreateUser(It.IsAny<Guid>())).ReturnsAsync(userId);

    var result = await _orchestrator.CreateUser();

    result.Should().Be(userId);
  }

  [Fact]
  public async Task DeleteUser_WhenCalled_ShouldReturnTrue()
  {
    var userId = Guid.NewGuid();
    _db.Setup(x => x.DeleteUser(userId)).ReturnsAsync(true);

    var result = await _orchestrator.DeleteUser(userId);

    result.Should().BeTrue();
  }

  [Fact]
  public async Task DeleteUser_WhenCalled_ShouldReturnFalse()
  {
    var userId = Guid.NewGuid();
    _db.Setup(x => x.DeleteUser(userId)).ReturnsAsync(false);

    var result = await _orchestrator.DeleteUser(userId);

    result.Should().BeFalse();
  }
}
