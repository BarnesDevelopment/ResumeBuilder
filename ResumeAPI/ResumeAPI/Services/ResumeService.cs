
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        using (var writer = new System.IO.StringWriter())
        {        
            separator.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        } 
    }

    private static string AddEmailToHeader(ResumeHeader header)
    {
        // if (!string.IsNullOrEmpty(header.Email))
        // {
        //     Label label = new Label("Email: " + header.Email, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
        //     page.Elements.Add(label); 
        // }
        return "";
    }
    
    private static string AddWebsiteToHeader(ResumeHeader header)
    {
        // if (!string.IsNullOrEmpty(header.Website))
        // {
        //     Label label = new Label("Website: " + header.Website, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
        //     page.Elements.Add(label); 
        // }
        return "";
    }
    
    private static string AddPhoneToHeader(ResumeHeader header)
    {
        // if (header.Phone != null)
        // {
        //     Label label = new Label("Phone: " + header.Phone.FormattedNumber, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
        //     page.Elements.Add(label); 
        // }
        return "";
    }

    private static string AddNameToHeader(ResumeHeader header)
    {
        // if (!string.IsNullOrEmpty(header.Name))
        // {
        //     Label label = new Label(header.Name, 0, y, maxWidth, 32, Font.HelveticaBold, 32, TextAlign.Center);
        //     page.Elements.Add(label);
        // }
        return "";
    }
}