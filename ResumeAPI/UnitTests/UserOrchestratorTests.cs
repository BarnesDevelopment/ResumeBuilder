using Moq;
using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace UnitTests;

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
    public async Task Login_Success()
    {
        var guid = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var username = "testuser";
        var key = "pass123";
        var user = new UserViewModel
        {
            Id = guid,
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
        _service.Setup(x => x.VerifyKey(user.Id, key)).ReturnsAsync(VerificationResult.Correct);
        _db.Setup(x => x.RetrieveCookie(guid)).ReturnsAsync((Cookie)null);
        _db.Setup(x => x.CreateCookie(user.Id)).ReturnsAsync(cookie);

        var actual = await _orchestrator.Login(username, key);

        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task Login_Success_OldCookie()
    {
        var guid = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();
        var username = "testuser";
        var key = "pass123";
        var user = new UserViewModel
        {
            Id = guid,
            Username = username
        };
        var oldCookie = new Cookie
        {
            Key = guid3.ToString(),
            UserId = guid.ToString(),
            Expiration = DateTime.Now.AddHours(1),
            Active = true
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
        _service.Setup(x => x.VerifyKey(user.Id, key)).ReturnsAsync(VerificationResult.Correct);
        _db.Setup(x => x.RetrieveCookie(guid)).ReturnsAsync(oldCookie);
        _db.Setup(x => x.DeactivateCookie(oldCookie.KeyGuid()));
        _db.Setup(x => x.CreateCookie(user.Id)).ReturnsAsync(cookie);

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
            Id = guid,
            Username = username
        };
        var expected = new LoginAttempt();
        
        _service.Setup(x => x.GetUser(username)).ReturnsAsync(user);
        _service.Setup(x => x.VerifyKey(user.Id, key)).ReturnsAsync(VerificationResult.Incorrect);

        var actual = await _orchestrator.Login(username, key);

        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task Login_NoPasswordStored()
    {
        var username = "testuser";
        var key = "pass123";
        var expected = new LoginAttempt();
        
        _service.Setup(x => x.GetUser(username)).ReturnsAsync((UserViewModel?)null);

        var actual = await _orchestrator.Login(username, key);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateAccount_ShouldReturnUserInfo()
    {
        var guid = Guid.NewGuid();
        var key = "pass123456";
        var userInput = new UserViewModel
        {
            Username = "test",
            FirstName = "first",
            LastName = "last",
            Email = "email@email.com"
        };
        var user = new User
        {
            Id = guid,
            Username = "test",
            FirstName = "first",
            LastName = "last",
            Email = "email@email.com",
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };
        
        _service.Setup(x => x.CreateUser(userInput)).ReturnsAsync(user);
        _service.Setup(x => x.CreateKey(guid, key));

        var actual = await _orchestrator.CreateAccount(userInput, key);

        actual.Should().BeEquivalentTo(user);
    }
}