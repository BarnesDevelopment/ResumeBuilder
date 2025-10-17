using Moq;
using ResumeAPI.Database;
using ResumeAPI.Services;

namespace UnitTests.Services;

public class UserServiceTests
{
    private readonly IUserService _service;
    private readonly Mock<IAuthClient> _authClient;

    public UserServiceTests()
    {
        _authClient = new Mock<IAuthClient>();
        _service = new UserService(_authClient.Object);
    }
    
    [Fact]
    public async Task CreateAnonymousUser_Success()
    {
        var userId = Guid.NewGuid();
        var jwt = "test-jwt";
        _authClient.Setup(a => a.CreateAnonymousUser()).ReturnsAsync(userId);
        _authClient.Setup(a => a.VendJwtFromId(userId)).ReturnsAsync(jwt);

        var (returnedJwt, returnedId) = await _service.CreateAnonymousUser();

        returnedJwt.Should().Be(jwt);
        returnedId.Should().Be(userId);
    }
    
    [Fact]
    public async Task CreateAnonymousUser_Failure()
    {
        _authClient.Setup(a => a.CreateAnonymousUser()).ReturnsAsync((Guid?)null);

        await Assert.ThrowsAsync<Exception>(async () => await _service.CreateAnonymousUser());
    }
    
    [Fact]
    public async Task DeleteAnonymousUser_Success()
    {
        var userId = Guid.NewGuid();
        _authClient.Setup(a => a.DeleteUser(userId)).ReturnsAsync(true);

        var result = await _service.DeleteAnonymousUser(userId);

        result.Should().BeTrue();
    }
}