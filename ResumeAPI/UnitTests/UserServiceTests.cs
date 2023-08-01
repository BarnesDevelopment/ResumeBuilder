using Moq;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace UnitTests;

public class UserServiceTests
{
    private readonly IUserService _service;
    private readonly Mock<IMySqlContext> _db;
    private readonly Mock<IPasswordHasher> _hasher;

    public UserServiceTests()
    {
        _db = new Mock<IMySqlContext>();
        _hasher = new Mock<IPasswordHasher>();
        _service = new UserService(_db.Object,_hasher.Object);
        
    }

    [Fact]
    public async Task GetUsers_ReturnsListFromDb()
    {
        var expected = new List<UserViewModel>();
        _db.Setup(x => x.GetUsers()).ReturnsAsync(expected);

        var actual = await _service.GetUsers();

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetUser_ReturnUserInfo()
    {
        var username = "test";
        var expected = new UserViewModel
        {
            Username = username
        };
        _db.Setup(x => x.GetUser(username)).ReturnsAsync(expected);

        var actual = await _service.GetUser(username);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateUser_ReturnUserInfo()
    {
        var guid = Guid.NewGuid();
        var user = new UserViewModel()
        {
            Username = "test",
            FirstName = "testname",
            LastName = "testlast",
            Email = "test@test.com"
        };

        var expected = new User
        {
            Username = "test",
            FirstName = "testname",
            LastName = "testlast",
            Email = "test@test.com",
            Id = guid.ToString(),
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };

        _db.Setup(x => x.CreateUser(It.IsAny<User>())).ReturnsAsync(expected);

        var actual = await _service.CreateUser(user);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateUser_ReturnUpdatedUserInfo()
    {
        var guid = Guid.NewGuid();
        var userInput = new UserViewModel
        {
            Id = guid.ToString(),
            Username = "test",
            FirstName = "testname",
            LastName = "testlast",
            Email = "test@test.com"
        };
        
        _db.Setup(x => x.UpdateUser(guid,userInput)).ReturnsAsync(userInput);

        var actual = await _service.UpdateUser(guid.ToString(),userInput);

        actual.Should().BeEquivalentTo(userInput);
    }

    [Fact]
    public async Task DeleteUser_ReturnSuccess()
    {
        var guid = Guid.NewGuid();
        _db.Setup(x => x.DeleteUser(guid)).ReturnsAsync(true);

        var actual = await _service.DeleteUser(guid.ToString());

        actual.Should().Be(true);
    }

    [Fact]
    public async Task VerifyKey_CorrectKey()
    {
        var guid = Guid.NewGuid();
        var key = "pass123";
        var hash = "123456789123";

        _db.Setup(x => x.RetrieveKey(guid)).ReturnsAsync(hash);
        _hasher.Setup(x => x.VerifyHashedPassword(hash, key)).Returns(PasswordVerificationResult.Success);

        var actual = await _service.VerifyKey(guid, key);

        actual.Should().Be(VerificationResult.Correct);
    }
    
    [Fact]
    public async Task VerifyKey_IncorrectKey()
    {
        var guid = Guid.NewGuid();
        var key = "pass123";
        var hash = "123456789123";

        _db.Setup(x => x.RetrieveKey(guid)).ReturnsAsync(hash);
        _hasher.Setup(x => x.VerifyHashedPassword(hash, key)).Returns(PasswordVerificationResult.Failed);

        var actual = await _service.VerifyKey(guid, key);

        actual.Should().Be(VerificationResult.Incorrect);
    }
    
    [Fact]
    public async Task VerifyKey_NoKeyFound()
    {
        var guid = Guid.NewGuid();
        var key = "pass123";
        var hash = "123456789123";

        _db.Setup(x => x.RetrieveKey(guid)).ReturnsAsync((string?)null);

        var actual = await _service.VerifyKey(guid, key);

        actual.Should().Be(VerificationResult.NotFound);
    }

    [Fact]
    public async Task CreateKey_ShouldCreateKey()
    {
        var guid = Guid.NewGuid();
        var key = "pass123";
        var hash = "123456";
        _hasher.Setup(x => x.HashPassword(key)).Returns(hash);
        _db.Setup(x => x.CreateKey(hash, guid)).ReturnsAsync(true);

        var actual = await _service.CreateKey(guid, key);

        actual.Should().Be(true);
    }
}