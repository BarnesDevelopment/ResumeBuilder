using System;
using System.Threading.Tasks;
using Moq;
using ResumeAPI.Helpers;
using ResumeAPI.Services;

namespace UnitTests.Helpers;

public class HelperTests
{
    [Fact]
    public async Task CookieValidator_Validate_NoUser()
    {
        var cookie = Guid.NewGuid();
        var mockService = new Mock<IUserService>();
        mockService.Setup(x => x.GetUserByCookie(cookie)).ReturnsAsync(Guid.Empty);
        var validator = new CookieValidator(mockService.Object);
        
        var actual = await validator.Validate(cookie.ToString());

        actual.Should().BeNull();
    }
    
    [Fact]
    public async Task CookieValidator_Validate_ExpiredCookie()
    {
        var id = Guid.NewGuid();
        var cookie = Guid.NewGuid();
        var mockService = new Mock<IUserService>();
        mockService.Setup(x => x.GetUserByCookie(cookie)).ReturnsAsync(id);
        mockService.Setup(x => x.VerifyCookie(id,cookie)).ReturnsAsync(false);
        var validator = new CookieValidator(mockService.Object);
        
        var actual = await validator.Validate(cookie.ToString());

        actual.Should().BeNull();
    }
    
    [Fact]
    public async Task CookieValidator_Validate_ValidCookie()
    {
        var id = Guid.NewGuid();
        var cookie = Guid.NewGuid();
        var mockService = new Mock<IUserService>();
        mockService.Setup(x => x.GetUserByCookie(cookie)).ReturnsAsync(id);
        mockService.Setup(x => x.VerifyCookie(id, cookie)).ReturnsAsync(true);
        var validator = new CookieValidator(mockService.Object);
        
        var actual = (Guid)(await validator.Validate(cookie.ToString()))!;

        actual.Should().Be(id);
    }
    
    [Fact]
    public async Task CookieValidator_Validate_NullCookie()
    {
        var mockService = new Mock<IUserService>();
        var validator = new CookieValidator(mockService.Object);
        
        var actual = await validator.Validate("");

        actual.Should().BeNull();
    }
}
