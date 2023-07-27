using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IResumeService
{
    TagBuilder BuildHeader(ResumeHeader header);
    TagBuilder BuildSummary(ResumeHeader header);
    TagBuilder BuildBody();
}

public class ResumeService : IResumeService
{
    private static readonly float margin = 20.0f;
    private static readonly int maxWidth = 572;
    
    public ResumeService()
    {
        
    }

    public TagBuilder BuildBody()
    {
        var body = new TagBuilder("body");
        
        var style = new TagBuilder("style");
        StreamReader sr = new StreamReader("./CSS/DefaultCss.css");
        var css = sr.ReadToEnd();
        sr.Close();
        style.InnerHtml.AppendHtml(css);
        
        body.InnerHtml.AppendHtml(style);
        
        return body;
    }

    public TagBuilder BuildHeader(ResumeHeader header)
    {
        var headerTag = new TagBuilder("div");
        headerTag.AddCssClass("header");
        headerTag.InnerHtml.AppendHtml(AddNameToHeader(header));
        headerTag.InnerHtml.AppendHtml(AddEmailToHeader(header));
        headerTag.InnerHtml.AppendHtml(AddPhoneToHeader(header));
        headerTag.InnerHtml.AppendHtml(AddWebsiteToHeader(header));
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

    private static TagBuilder AddEmailToHeader(ResumeHeader header)
    {
        if (!string.IsNullOrEmpty(header.Email))
        {
            var email = new TagBuilder("span");
            email.AddCssClass("email");
            email.InnerHtml.Append("Email: " + header.Email);
            return email;
        }
        return new TagBuilder("span");
    }
    
    private static TagBuilder AddWebsiteToHeader(ResumeHeader header)
    {
        if (!string.IsNullOrEmpty(header.Website))
        {
            var website = new TagBuilder("span");
            website.AddCssClass("website");
            website.InnerHtml.Append("Website: " + header.Website);
            return website;
        }
        return new TagBuilder("span");
    }
    
    private static TagBuilder AddPhoneToHeader(ResumeHeader header)
    {
        if (header.Phone != null)
        {
            var phone = new TagBuilder("span");
            phone.AddCssClass("phone");
            phone.InnerHtml.Append("Phone: " + header.Phone.FormattedNumber);
            return phone;
        }
        return new TagBuilder("span");
    }

    private static TagBuilder AddNameToHeader(ResumeHeader header)
    {
        if (!string.IsNullOrEmpty(header.Name))
        {
            var name = new TagBuilder("span");
            name.AddCssClass("name");
            name.InnerHtml.Append(header.Name);
            return name;
        }
        return new TagBuilder("span");
    }
}