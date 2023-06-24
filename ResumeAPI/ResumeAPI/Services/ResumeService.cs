using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IResumeService
{
    Page AddPage(Document doc);
    int BuildHeader(Page page, ResumeHeader header, int y = 0);
    int AddSeparator(Page page, int y, string title);
}

public class ResumeService : IResumeService
{
    private static readonly float margin = 20.0f;
    private static readonly int maxWidth = 572;
    
    public ResumeService()
    {
        
    }
    public Page AddPage(Document doc)
    {
        Page page = new Page(PageSize.Letter, PageOrientation.Portrait, margin);
        doc.Pages.Add(page);
        return page;
    }

    public int BuildHeader(Page page, ResumeHeader header, int y = 0)
    {
        var padding = 2;
        AddNameToHeader(page, header, y);
        y += 32 + padding;
        AddEmailToHeader(page, header, y);
        y += 12 + padding;
        AddPhoneToHeader(page, header, y);
        y += 12 + padding;
        AddWebsiteToHeader(page, header, y);
        y += 24;
        return y;
    }

    public int AddSeparator(Page page, int y, string title)
    {
        var sector = maxWidth / 5;
        Line line1 = new Line(0,y+10,sector*2,y+10,1,RgbColor.Black,LineStyle.Solid);
        page.Elements.Add(line1);
        Label label = new Label(title, sector*2, y, sector, 18, Font.HelveticaBold, 18, TextAlign.Center);
        page.Elements.Add(label); 
        Line line2 = new Line(sector*3,y+10,maxWidth,y+10,1,RgbColor.Black,LineStyle.Solid);
        page.Elements.Add(line2);
        return y + 18;
    }

    private static void AddEmailToHeader(Page page, ResumeHeader header, int y)
    {
        if (!string.IsNullOrEmpty(header.Email))
        {
            Label label = new Label("Email: " + header.Email, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
            page.Elements.Add(label); 
        }
    }
    
    private static void AddWebsiteToHeader(Page page, ResumeHeader header, int y)
    {
        if (!string.IsNullOrEmpty(header.Website))
        {
            Label label = new Label("Website: " + header.Website, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
            page.Elements.Add(label); 
        }
    }
    
    private static void AddPhoneToHeader(Page page, ResumeHeader header, int y)
    {
        if (header.Phone != null)
        {
            Label label = new Label("Phone: " + header.Phone.FormattedNumber, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
            page.Elements.Add(label); 
        }
    }

    private static void AddNameToHeader(Page page, ResumeHeader header, int y)
    {
        if (!string.IsNullOrEmpty(header.Name))
        {
            Label label = new Label(header.Name, 0, y, maxWidth, 32, Font.HelveticaBold, 32, TextAlign.Center);
            page.Elements.Add(label);
        }
    }
}