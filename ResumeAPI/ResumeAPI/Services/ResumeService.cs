using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Helpers;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IResumeService
{
    string BuildHeader(ResumeHeader header);
    string BuildSummary(ResumeHeader header);
}

public class ResumeService : IResumeService
{
    private static readonly float margin = 20.0f;
    private static readonly int maxWidth = 572;
    
    public ResumeService()
    {
        
    }

    public string BuildHeader(ResumeHeader header)
    {
        var html = "";
        html += AddNameToHeader(header);
        html += AddEmailToHeader(header);
        html += AddPhoneToHeader(header);
        html += AddWebsiteToHeader(header);
        return html;
    }

    public string BuildSummary(ResumeHeader header)
    {
        var html = "";
        html += AddSeparator("Summary");
        
        // TextArea text = new TextArea(header.Summary, 0, y, maxWidth, 12, Font.HelveticaBold, 18, TextAlign.Center);
        // page.Elements.Add(text);
        //
        // y += 14;
        
        return html;
    }

    private string AddSeparator(string title)
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

        return separator.Write();
    }

    private static string AddEmailToHeader(ResumeHeader header)
    {
        if (!string.IsNullOrEmpty(header.Email))
        {
            var email = new TagBuilder("span");
            email.AddCssClass("email");
            email.InnerHtml.Append("Email: " + header.Email);
            return email.Write();
        }
        return "";
    }
    
    private static string AddWebsiteToHeader(ResumeHeader header)
    {
        if (!string.IsNullOrEmpty(header.Website))
        {
            var website = new TagBuilder("span");
            website.AddCssClass("website");
            website.InnerHtml.Append("Website: " + header.Website);
            return website.Write();
        }
        return "";
    }
    
    private static string AddPhoneToHeader(ResumeHeader header)
    {
        if (header.Phone != null)
        {
            var phone = new TagBuilder("span");
            phone.AddCssClass("phone");
            phone.InnerHtml.Append("Phone: " + header.Phone.FormattedNumber);
            return phone.Write();
        }
        return "";
    }

    private static string AddNameToHeader(ResumeHeader header)
    {
        if (!string.IsNullOrEmpty(header.Name))
        {
            var name = new TagBuilder("span");
            name.AddCssClass("name");
            name.InnerHtml.Append(header.Name);
            return name.Write();
        }
        return "";
        
        
    }
}