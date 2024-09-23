using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace UnitTests.Services;

public class ResumeBuilderBuilderServiceTests
{
  private readonly IResumeBuilderService _builderService = new ResumeBuilderBuilderService();

  [Fact]
  public void BuildSummary_AddsText()
  {
    var text = "summary text";
    var header = new ResumeHeader
    {
      Summary = text
    };

    var actual = _builderService.BuildSummary(header);
    actual.Attributes["class"].Should().Be("paragraph");
    actual.Write().Should().Contain(text);
  }

  [Fact]
  public void VerticalSeparator_CreatesText()
  {
    var actual = _builderService.VerticalSeparator();
    actual.Attributes["class"].Should().Be("vertical-separator");
    actual.Write().Should().Contain("|");
  }
}
