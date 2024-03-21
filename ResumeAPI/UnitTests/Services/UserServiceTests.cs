using Moq;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace UnitTests.Services;

public class UserServiceTests
{
  private readonly IUserService _service;
  private readonly Mock<IUserData> _db;

  public UserServiceTests()
  {
    _db = new Mock<IUserData>();
    _service = new UserService(_db.Object);
  }

  [Fact]
  public async Task CreateUser_ReturnUserId()
  {
    var guid = Guid.NewGuid();
    _db.Setup(x => x.CreateUser(guid)).ReturnsAsync(guid);

    var actual = await _service.CreateUser(guid);

    actual.Should().Be(guid);
  }

  [Fact]
  public async Task DeleteUser_ReturnSuccess()
  {
    var guid = Guid.NewGuid();
    _db.Setup(x => x.DeleteUser(guid)).ReturnsAsync(true);

    var actual = await _service.DeleteUser(guid.ToString());

    actual.Should().Be(true);
  }
}
