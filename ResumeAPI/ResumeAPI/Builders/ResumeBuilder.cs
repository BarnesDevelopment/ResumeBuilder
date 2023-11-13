using Microsoft.AspNetCore.Mvc.Rendering;
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
    titleBlock.InnerHtml.AppendHtml(TagHelper.CreatTag("div", "name", node.Content));
    var verticalSeparator = TagHelper.CreatTag("span", "vertical-separator","|");
    var details = TagHelper.CreatTag("div", "details");
    details.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "email", node.Children[0].Content));
    details.InnerHtml.AppendHtml(verticalSeparator);
    details.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "phone", node.Children[1].Content));
    if (node.Children.Count > 2 && !string.IsNullOrEmpty(node.Children[2].Content))
    {
      details.InnerHtml.AppendHtml(verticalSeparator);
      details.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "website", node.Children[2].Content));
    }
    titleBlock.InnerHtml.AppendHtml(details);
    return titleBlock;
  }
  
  private static TagBuilder BuildSection(ResumeTreeNode node)
  {
    var section = TagHelper.CreatTag("div", "section");
    section.InnerHtml.AppendHtml(AddSeparator(node.Content));
    var child = node.Children[0];
    switch (child.NodeType)
    {
      case ResumeNodeType.Paragraph:
        section.AddCssClass("paragraph");
        section.InnerHtml.AppendHtml(TagHelper.CreatTag("p", "", child.Content));
        break;
      case ResumeNodeType.List:
        section.AddCssClass("list");
        section.InnerHtml.AppendHtml(BuildList(child.Children));
        break;
      case ResumeNodeType.WorkExperience:
        BuildWorkExperience(section, node.Children);
        break;
    }
    return section;
  }

  private static void BuildWorkExperience(TagBuilder section, List<ResumeTreeNode> nodes)
  {
    foreach (var node in nodes)
    {
      var job = TagHelper.CreatTag("div", "job");
      var header = TagHelper.CreatTag("div", "job-header");
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "title", node.Children[0].Content));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "vertical-separator", "|"));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "employer", node.Children[1].Content));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "spacer", "-"));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "city", node.Children[2].Content));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "comma", ","));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "state", node.Children[3].Content));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "vertical-separator", "|"));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "start", node.Children[4].Content));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "spacer", "-"));
      header.InnerHtml.AppendHtml(TagHelper.CreatTag("span", "end", node.Children[5].Content));
      job.InnerHtml.AppendHtml(header);

      if (node.Children.Count == 7)
      {
        var responsibilities = TagHelper.CreatTag("div", "responsibilities");
        var ul = TagHelper.CreatTag("ul", "list");
        foreach (var responsibility in node.Children[6].Children)
        {
          ul.InnerHtml.AppendHtml(TagHelper.CreatTag("li", "", responsibility.Content));
        }

        responsibilities.InnerHtml.AppendHtml(ul);
        job.InnerHtml.AppendHtml(responsibilities);
      }

      section.InnerHtml.AppendHtml(job);
    }
  }
  
  private static TagBuilder BuildList(List<ResumeTreeNode> nodes)
  {
    
    var row = TagHelper.CreatTag("div", "row");
    row.InnerHtml.AppendHtml(TagHelper.CreatTag("div","spacer"));
    var cell = TagHelper.CreatTag("div", "cell");
    var ul = TagHelper.CreatTag("ul", "");
    foreach (var node in nodes)
    {
      ul.InnerHtml.AppendHtml(TagHelper.CreatTag("li", "", node.Content));
    }
    row.InnerHtml.AppendHtml(cell);
    cell.InnerHtml.AppendHtml(ul);
    var container = TagHelper.CreatTag("div", "container");
    container.InnerHtml.AppendHtml(row);
    return container;
  }
  
  private static TagBuilder AddSeparator(string title)
  {
    var hr = new TagBuilder("div");
    hr.InnerHtml.AppendHtml(new TagBuilder("div"));
    hr.AddCssClass("separator");
        
    var textSpan = new TagBuilder("span");
    textSpan.InnerHtml.Append(title);
    var text = new TagBuilder("div");
    text.InnerHtml.AppendHtml(textSpan);
    text.AddCssClass("separator-text");
        
    var separator = new TagBuilder("div");
    separator.AddCssClass("separator-container");
    var separatorTable = new TagBuilder("div");
    separatorTable.AddCssClass("separator-table");
    separator.InnerHtml.AppendHtml(hr);
    separator.InnerHtml.AppendHtml(text);
    separator.InnerHtml.AppendHtml(hr);
        
    separatorTable.InnerHtml.AppendHtml(separator);

    return separatorTable;
  }
}
