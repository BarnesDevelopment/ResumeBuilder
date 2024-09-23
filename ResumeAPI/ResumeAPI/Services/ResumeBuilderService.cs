using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IResumeBuilderService
{
  TagBuilder BuildSummary(ResumeHeader header);
  TagBuilder BuildBody(List<TagBuilder> pages);
  TagBuilder NewPage(int newPageId);
  TagBuilder VerticalSeparator();
  TagBuilder AddSeparator(string title);
  TagBuilder BuildStyle();
}

public class ResumeBuilderBuilderService : IResumeBuilderService
{
  public TagBuilder BuildBody(List<TagBuilder> pages)
  {
    var body = new TagBuilder("body");

    foreach (var page in pages) body.InnerHtml.AppendHtml(page);

    return body;
  }

  public TagBuilder BuildStyle()
  {
    var head = new TagBuilder("head");
    var style = new TagBuilder("style");
    var sr = new StreamReader("./CSS/DefaultCss.css");
    var css = sr.ReadToEnd();
    sr.Close();
    style.InnerHtml.AppendHtml(css);
    head.InnerHtml.AppendHtml(style);
    return head;
  }

  public TagBuilder BuildSummary(ResumeHeader header)
  {
    var summary = new TagBuilder("div");
    summary.AddCssClass("paragraph");
    var separator = AddSeparator("Summary");
    summary.InnerHtml.AppendHtml(separator);
    var text = new TagBuilder("p");
    text.InnerHtml.Append(header.Summary);
    summary.InnerHtml.AppendHtml(text);

    return summary;
  }

  public TagBuilder NewPage(int newPageId)
  {
    var page = new TagBuilder("div");
    page.AddCssClass("page");
    page.GenerateId($"page{newPageId}", "");
    return page;
  }

  public TagBuilder AddSeparator(string title)
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

  public TagBuilder VerticalSeparator()
  {
    return TagHelper.CreatTag("span", "vertical-separator", "|");
  }
}
