using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IResumeService
{
    Page AddPage(Document doc);
    void BuildHeader(Page page, ResumeHeader header);
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

    public void BuildHeader(Page page, ResumeHeader header)
    {
        var y = 0;
        var padding = 2;
        AddNameToHeader(page, header, y);
        y += 32 + padding;
        AddEmailToHeader(page, header, y);
        y += 12 + padding;
        AddPhoneToHeader(page, header, y);
        y += 12 + padding;
        AddWebsiteToHeader(page, header, y);
        y += 24;
        AddSeparator(page, y);
    }

    private static void AddSeparator(Page page, int y)
    {
        Line line = new Line(0,y,maxWidth,y,1,RgbColor.Black,LineStyle.Solid);
        page.Elements.Add(line);
    }

    private static void AddEmailToHeader(Page page, ResumeHeader header, int y)
    {
        if (!string.IsNullOrEmpty(header.Email))
        {
            Label label = new Label(header.Email, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
            page.Elements.Add(label); 
        }
    }
    
    private static void AddWebsiteToHeader(Page page, ResumeHeader header, int y)
    {
        if (!string.IsNullOrEmpty(header.Website))
        {
            Label label = new Label(header.Website, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
            page.Elements.Add(label); 
        }
    }
    
    private static void AddPhoneToHeader(Page page, ResumeHeader header, int y)
    {
        if (header.Phone != null)
        {
            Label label = new Label(header.Phone.FormattedNumber, 0, y, maxWidth, 12, Font.Helvetica, 12, TextAlign.Center);
            page.Elements.Add(label); 
        }
    }

    private static void AddNameToHeader(Page page, ResumeHeader header, int y)
    {
        if (!string.IsNullOrEmpty(header.Name))
        {
            Label label = new Label(header.Name, 0, y, maxWidth, 32, Font.Helvetica, 32, TextAlign.Center);
            page.Elements.Add(label);
        }
    }
}