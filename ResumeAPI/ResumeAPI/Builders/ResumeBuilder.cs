using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Builders;

public static class ResumeBuilder
{
  public static string Build(this ResumeTreeNode tree)
  {
    var html = new TagBuilder("html");
    html.InnerHtml.AppendHtml(BuildStyle());
    html.InnerHtml.AppendHtml(BuildBody(tree));
    return html.Write();
  }

  private static TagBuilder BuildStyle()
  {
    var head = new TagBuilder("head");
    var style = new TagBuilder("style");
    StreamReader sr = new StreamReader("./CSS/DefaultCss.css");
    var css = sr.ReadToEnd();
    sr.Close();
    style.InnerHtml.AppendHtml(css);
    head.InnerHtml.AppendHtml(style);
    return head;
  }

  private static TagBuilder BuildBody(ResumeTreeNode tree)
  {
    var body =  new TagBuilder("body");
    
    for(var i = 0; i < tree.Children.Count; i++)
    {
      if (i == 0)
      {
        body.InnerHtml.AppendHtml(BuildTitleBlock(tree.Children[i]));
      }
      else
      {
        body.InnerHtml.AppendHtml(BuildSection(tree.Children[i]));
      }
    }
    
    return body;
  }
  
  private static TagBuilder BuildTitleBlock(ResumeTreeNode node)
  {
    var titleBlock = new TagBuilder("div");
    titleBlock.AddCssClass("header");
    titleBlock.InnerHtml.AppendHtml(TagHelper.CreatTag("div", "name", node.Children[0].Content));
    var verticalSeparator = TagHelper.CreatTag("span", "vertical-separator","|");
    var details = TagHelper.CreatTag("div", "details");
    details.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "email", node.Children[1].Content));
    details.InnerHtml.AppendHtml(verticalSeparator);
    details.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "phone", node.Children[2].Content));
    if (node.Children.Count > 3 && string.IsNullOrEmpty(node.Children[3].Content))
    {
      details.InnerHtml.AppendHtml(verticalSeparator);
      details.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "website", node.Children[3].Content));
    }
    titleBlock.InnerHtml.AppendHtml(details);
    return titleBlock;
  }
  
  private static TagBuilder BuildSection(ResumeTreeNode node)
  {
    var section = new TagBuilder("div");
    
    return section;
  }
}
