using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace UnitTests;

public class ResumeServiceTests
{
    private readonly IResumeService _service = new ResumeService();

    [Fact]
    public void BuildSummary_AddsText()
    {
        var text = "summary text";
        var header = new ResumeHeader
        {
            Summary = text
        };
    
        var actual = _service.BuildSummary(header);
        actual.Attributes["class"].Should().Be("paragraph");
        actual.Write().Should().Contain(text);
    }
    
    [Fact]
    public void VerticalSeparator_CreatesText()
    {
    
        var actual = _service.VerticalSeparator();
        actual.Attributes["class"].Should().Be("vertical-separator");
        actual.Write().Should().Contain("|");
    }
}
