using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using ResumeAPI.Helpers;
using ResumeAPI.Services;

namespace UnitTests;

public class HelperTests
{
    [Fact]
    public void TagHelper_Write_CreateCorrectString()
    {
        var expected = @"<div class=""className""></div>";
        var div = new TagBuilder("div");
        div.AddCssClass("className");

        var actual = div.Write();

        actual.Should().Be(expected);
    }

    [Fact]
    public void TagHelper_CreateTag_SetTagName()
    {
        var expected = @"<div></div>";
        
        var actual = TagHelper.CreatTag("div");

        actual.TagName.Should().Be("div");
        actual.Write().Should().Be(expected);
    }

    [Fact]
    public void TagHelper_CreateTag_SetClassName()
    {
        var expected = @"<div class=""some-class-name""></div>";
        
        var actual = TagHelper.CreatTag("div", "some-class-name");

        actual.Write().Should().Be(expected);
    }

    [Fact]
    public void TagHelper_CreateTag_SetInnerText()
    {
        var expected = @"<span>some inner text</span>";
        var someInnerText = "some inner text";
        
        var actual = TagHelper.CreatTag("span", "", someInnerText);

        actual.Write().Should().Be(expected);
    }
    
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