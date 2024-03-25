using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Helpers;

namespace UnitTests.Helpers;

public class TagHelperTests
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
}
