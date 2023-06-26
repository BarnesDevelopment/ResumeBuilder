using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
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
        
        TextArea text = new TextArea(header.Summary, 0, y, maxWidth, 12, Font.HelveticaBold, 18, TextAlign.Center);
        page.Elements.Add(text);

        y += 14;
        
        return html;
    }

    private string AddSeparator(string title)
    {
        var sector = maxWidth / 5;
        Line line1 = new Line(0,y+10,sector*2,y+10,1,RgbColor.Black,LineStyle.Solid);
        page.Elements.Add(line1);
        Label label = new Label(title, sector*2, y, sector, 18, Font.HelveticaBold, 18, TextAlign.Center);
        page.Elements.Add(label); 
        Line line2 = new Line(sector*3,y+10,maxWidth,y+10,1,RgbColor.Black,LineStyle.Solid);
        page.Elements.Add(line2);
        return html;
    }

    private static string AddEmailToHeader(ResumeHeader header)
    {
        if (!string.IsNullOrEmpty(header.Email))
        {
            Label label = new Label("Email: " + header.Email, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
            page.Elements.Add(label); 
        }
    }
    
    private static string AddWebsiteToHeader(ResumeHeader header)
    {
        if (!string.IsNullOrEmpty(header.Website))
        {
            Label label = new Label("Website: " + header.Website, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
            page.Elements.Add(label); 
        }
    }
    
    private static string AddPhoneToHeader(ResumeHeader header)
    {
        if (header.Phone != null)
        {
            Label label = new Label("Phone: " + header.Phone.FormattedNumber, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
            page.Elements.Add(label); 
        }
    }

    private static string AddNameToHeader(ResumeHeader header)
    {
        if (!string.IsNullOrEmpty(header.Name))
        {
            Label label = new Label(header.Name, 0, y, maxWidth, 32, Font.HelveticaBold, 32, TextAlign.Center);
            page.Elements.Add(label);
        }
    }
}