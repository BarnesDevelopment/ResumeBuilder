using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace UnitTests;

public class ResumeServiceTests
{
    private readonly IResumeService _serivce;

    public ResumeServiceTests()
    {
        _serivce = new ResumeService();
    }
    
    [Fact]
    public void NewPage_SetsIdAndClass_ReturnsPage()
    {
        
        var actual = _serivce.NewPage(0);

        actual.TagName.Should().Be("div");
        actual.Attributes["id"].Should().Be("page0");
        actual.Attributes["class"].Should().Be("page");
    }
    
    #region BuildHeader

    [Fact]
    public void CreateSpan_SetsTextAndClass()
    {
        var text = "some text";
        var className = "class";

        var actual = _serivce.CreateSpan(text, className);
        actual.TagName.Should().Be("span");
        actual.Attributes["class"].Should().Be(className);
        actual.Write().Should().Contain(text);
    }
    
    // [Fact]
    // public void BuildHeader_AddsName()
    // {
    //     var doc = new Document();
    //     var page = _serivce.AddPage(doc);
    //     var name = "Some Name";
    //     var header = new ResumeHeader
    //     {
    //         Name = name
    //     };
    //
    //     _serivce.BuildHeader(page, header);
    //
    //     ((Label)page.Elements[0]).Text.Should().Be(name);
    // }
    //
    // [Fact]
    // public void BuildHeader_AddsEmail()
    // {
    //     var doc = new Document();
    //     var page = _serivce.AddPage(doc);
    //     var email = "email@email.com";
    //     var header = new ResumeHeader
    //     {
    //         Email = email
    //     };
    //
    //     _serivce.BuildHeader(page, header);
    //
    //     ((Label)page.Elements[0]).Text.Should().Be($"Email: {email}");
    // }
    //
    // [Fact]
    // public void BuildHeader_AddsPhone()
    // {
    //     var doc = new Document();
    //     var page = _serivce.AddPage(doc);
    //     var phone = "(641) 555-5555";
    //     var header = new ResumeHeader
    //     {
    //         Phone = new PhoneNumber
    //         {
    //             AreaCode = 641,
    //             Prefix = 555,
    //             LineNumber = 5555
    //         }
    //     };
    //
    //     _serivce.BuildHeader(page, header);
    //
    //     ((Label)page.Elements[0]).Text.Should().Be($"Phone: {phone}");
    // }
    //
    // [Fact]
    // public void BuildHeader_AddsWebsite()
    // {
    //     var doc = new Document();
    //     var page = _serivce.AddPage(doc);
    //     var site = "www.site.com";
    //     var header = new ResumeHeader
    //     {
    //         Website = site
    //     };
    //
    //     _serivce.BuildHeader(page, header);
    //
    //     ((Label)page.Elements[0]).Text.Should().Be($"Website: {site}");
    // }
    //
    // [Fact]
    // public void BuildHeader_DataOrderedCorrectly()
    // {
    //     var doc = new Document();
    //     var page = _serivce.AddPage(doc);
    //     var name = "Some Name";
    //     var site = "www.site.com";
    //     var phone = "(641) 555-5555";
    //     var email = "email@email.com";
    //     var header = new ResumeHeader
    //     {
    //         Name = name,
    //         Website = site,
    //         Email = email,
    //         Phone = new PhoneNumber
    //         {
    //             AreaCode = 641,
    //             Prefix = 555,
    //             LineNumber = 5555
    //         }
    //     };
    //
    //     var y = _serivce.BuildHeader(page, header);
    //     
    //     ((Label)page.Elements[0]).Text.Should().Be(name);
    //     ((Label)page.Elements[1]).Text.Should().Be($"Email: {email}");
    //     ((Label)page.Elements[2]).Text.Should().Be($"Phone: {phone}");
    //     ((Label)page.Elements[3]).Text.Should().Be($"Website: {site}");
    //     y.Should().Be(86);
    // }
    
    #endregion
    
    #region AddSummary
    
    [Fact]
    public void BuildSummary_AddsText()
    {
        var text = "summary text";
        var header = new ResumeHeader
        {
            Summary = text
        };
    
        var actual = _serivce.BuildSummary(header);
        actual.Attributes["class"].Should().Be("summary");
        actual.Write().Should().Contain(text);
    }
    
    #endregion
}