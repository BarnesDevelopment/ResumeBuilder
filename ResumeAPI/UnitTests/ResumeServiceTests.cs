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
    
    [Fact]
    public void VerticalSeparator_CreatesText()
    {
    
        var actual = _serivce.VerticalSeparator();
        actual.Attributes["class"].Should().Be("vertical-separator");
        actual.Write().Should().Contain("|");
    }
}