using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
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
    public void AddPage_AddsPageToDocAndReturnsPage()
    {
        var doc = new Document();
        _serivce.AddPage(doc);

        doc.Pages.Count.Should().Be(1);
    }

    #region BuildHeader

    [Fact]
    public void BuildHeader_AddsName()
    {
        var doc = new Document();
        var page = _serivce.AddPage(doc);
        var name = "Some Name";
        var header = new ResumeHeader
        {
            Name = name
        };

        _serivce.BuildHeader(page, header);

        ((Label)page.Elements[0]).Text.Should().Be(name);
    }
    
    [Fact]
    public void BuildHeader_AddsEmail()
    {
        var doc = new Document();
        var page = _serivce.AddPage(doc);
        var email = "email@email.com";
        var header = new ResumeHeader
        {
            Email = email
        };

        _serivce.BuildHeader(page, header);

        ((Label)page.Elements[0]).Text.Should().Be($"Email: {email}");
    }
    
    [Fact]
    public void BuildHeader_AddsPhone()
    {
        var doc = new Document();
        var page = _serivce.AddPage(doc);
        var phone = "(641) 555-5555";
        var header = new ResumeHeader
        {
            Phone = new PhoneNumber
            {
                AreaCode = 641,
                Prefix = 555,
                LineNumber = 5555
            }
        };

        _serivce.BuildHeader(page, header);

        ((Label)page.Elements[0]).Text.Should().Be($"Phone: {phone}");
    }
    
    [Fact]
    public void BuildHeader_AddsWebsite()
    {
        var doc = new Document();
        var page = _serivce.AddPage(doc);
        var site = "www.site.com";
        var header = new ResumeHeader
        {
            Website = site
        };

        _serivce.BuildHeader(page, header);

        ((Label)page.Elements[0]).Text.Should().Be($"Website: {site}");
    }

    [Fact]
    public void BuildHeader_DataOrderedCorrectly()
    {
        var doc = new Document();
        var page = _serivce.AddPage(doc);
        var name = "Some Name";
        var site = "www.site.com";
        var phone = "(641) 555-5555";
        var email = "email@email.com";
        var header = new ResumeHeader
        {
            Name = name,
            Website = site,
            Email = email,
            Phone = new PhoneNumber
            {
                AreaCode = 641,
                Prefix = 555,
                LineNumber = 5555
            }
        };

        _serivce.BuildHeader(page, header);
        
        ((Label)page.Elements[0]).Text.Should().Be(name);
        ((Label)page.Elements[1]).Text.Should().Be($"Email: {email}");
        ((Label)page.Elements[2]).Text.Should().Be($"Phone: {phone}");
        ((Label)page.Elements[3]).Text.Should().Be($"Website: {site}");
    }

    #endregion

    #region AddSeparator

    [Fact]
    public void AddSeparator_AddsText()
    {
        var doc = new Document();
        var page = _serivce.AddPage(doc);
        var y = 0;
        var title = "title";

        _serivce.AddSeparator(page, y, title);

        ((Label)page.Elements[1]).Text.Should().Be(title);
    }

    #endregion
}