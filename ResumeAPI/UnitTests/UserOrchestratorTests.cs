using Moq;
using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace UnitTests;

public class UserOrchestratorTests
{
    private readonly Mock<IUserService> _service;
    private readonly Mock<IMySqlContext> _db;
    private readonly IUserOrchestrator _orchestrator;

    public UserOrchestratorTests()
    {
        _service = new Mock<IUserService>();
        _db = new Mock<IMySqlContext>();
        _orchestrator = new UserOrchestrator(_service.Object, _db.Object);
    }

    [Fact]
    public async Task Login_Success()
    {
        var guid = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var username = "testuser";
        var key = "pass123";
        var user = new UserViewModel
        {
            Id = guid.ToString(),
            Username = username
        };
        var cookie = new Cookie
        {
            Active = true,
            Key = guid2.ToString(),
            Expiration = DateTime.Today.AddDays(1),
            UserId = guid.ToString()
        };
        var expected = new LoginAttempt(cookie);
        
        _service.Setup(x => x.GetUser(username)).ReturnsAsync(user);
        _service.Setup(x => x.VerifyKey(user.IdGuid(), key)).ReturnsAsync(VerificationResult.Correct);
        _db.Setup(x => x.CreateCookie(user.IdGuid())).ReturnsAsync(cookie);

        var actual = await _orchestrator.Login(username, key);

        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task Login_WrongPassword()
    {
        var guid = Guid.NewGuid();
        var username = "testuser";
        var key = "pass123";
        var user = new UserViewModel
        {
            Id = guid.ToString(),
            Username = username
        };
        var expected = new LoginAttempt();
        
        _service.Setup(x => x.GetUser(username)).ReturnsAsync(user);
        _service.Setup(x => x.VerifyKey(user.IdGuid(), key)).ReturnsAsync(VerificationResult.Incorrect);

        var actual = await _orchestrator.Login(username, key);

        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task Login_NoPasswordStored()
    {
        var guid = Guid.NewGuid();
        var username = "testuser";
        var key = "pass123";
        var expected = new LoginAttempt();
        
        _service.Setup(x => x.GetUser(username)).ReturnsAsync((UserViewModel?)null);

        var actual = await _orchestrator.Login(username, key);

        actual.Should().BeEquivalentTo(expected);
    }
}