using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IResumeService
{
    TagBuilder BuildHeader(ResumeHeader header);
    TagBuilder BuildSummary(ResumeHeader header);
    TagBuilder BuildBody(List<TagBuilder> pages);
    TagBuilder NewPage(int newPageId);
    TagBuilder CreateSpan(string text, string className);
}

public class ResumeService : IResumeService
{
    private static readonly float margin = 20.0f;
    private static readonly int maxWidth = 572;
    
    public ResumeService()
    {
        
    }

    public TagBuilder BuildBody(List<TagBuilder> pages)
    {
        var body = new TagBuilder("body");
        
        var style = BuildStyle();

        body.InnerHtml.AppendHtml(style);

        foreach (var page in pages)
        {
            body.InnerHtml.AppendHtml(page);
        }
        
        return body;
    }

    private TagBuilder BuildStyle()
    {
        var style = new TagBuilder("style");
        StreamReader sr = new StreamReader("./CSS/DefaultCss.css");
        var css = sr.ReadToEnd();
        sr.Close();
        style.InnerHtml.AppendHtml(css);
        return style;
    }

    public TagBuilder BuildHeader(ResumeHeader header)
    {
        var headerTag = new TagBuilder("div");
        headerTag.AddCssClass("header");
        if(!string.IsNullOrEmpty(header.Name)) headerTag.InnerHtml.AppendHtml(CreateSpan(header.Name, "name"));
        if(!string.IsNullOrEmpty(header.Email)) headerTag.InnerHtml.AppendHtml(CreateSpan(header.Email, "email"));
        if(header.Phone != null) headerTag.InnerHtml.AppendHtml(CreateSpan(header.Phone.FormattedNumber, "phone"));
        if(!string.IsNullOrEmpty(header.Website)) headerTag.InnerHtml.AppendHtml(CreateSpan(header.Website, "website"));
        return headerTag;
    }

    public TagBuilder BuildSummary(ResumeHeader header)
    {
        var summary = new TagBuilder("div");
        summary.AddCssClass("summary");
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

    private TagBuilder AddSeparator(string title)
    {
        var sector = maxWidth / 5;
        var hr = new TagBuilder("div");
        hr.InnerHtml.AppendHtml("<hr>");
        hr.AddCssClass("separator");
        
        var textSpan = new TagBuilder("span");
        textSpan.InnerHtml.Append(title);
        var text = new TagBuilder("div");
        text.InnerHtml.AppendHtml(textSpan);
        text.AddCssClass("text");
        
        var separator = new TagBuilder("div");
        separator.AddCssClass("separator-container");
        separator.InnerHtml.AppendHtml(hr);
        separator.InnerHtml.AppendHtml(text);
        separator.InnerHtml.AppendHtml(hr);

        return separator;
    }

    public TagBuilder CreateSpan(string text, string className)
    {
            var span = new TagBuilder("span");
            span.AddCssClass(className);
            span.InnerHtml.Append(text);
            return span;
    }
}