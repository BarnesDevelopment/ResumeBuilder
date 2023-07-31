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