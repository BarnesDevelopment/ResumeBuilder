using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;
using SelectPdf;

namespace ResumeAPI.Orchestrator;

public interface IResumeOrchestrator
{
    MemoryStream BuildResume(Resume resume);
    string BuildResumeHtml(Resume resume);
}

public class ResumeOrchestrator : IResumeOrchestrator
{
    private readonly IResumeService _service;
    
    public ResumeOrchestrator(IResumeService service)
    {
        _service = service;
    }
    public MemoryStream BuildResume(Resume header)
    {
        var stream = new MemoryStream();
        HtmlToPdf converter = new HtmlToPdf();
        converter.Options.PdfPageSize = PdfPageSize.Letter;
        
        var body = CreateHtml(header);

        PdfDocument doc = converter.ConvertHtmlString(body.Write());
        doc.Save(stream);
        doc.Close();
        return stream;
    }

    public string BuildResumeHtml(Resume resume)
    {
        var html = new TagBuilder("html");
        html.InnerHtml.AppendHtml(CreateHtml(resume));
        return(html.Write());
    }

    private TagBuilder CreateHtml(Resume resume)
    {
        var page0 = _service.NewPage(0);
        page0.InnerHtml.AppendHtml(BuildHeader(resume.Header));
        page0.InnerHtml.AppendHtml(_service.BuildSummary(resume.Header));

        var body = _service.BuildBody(new List<TagBuilder> { page0 });
        return body;
    }
    
    public TagBuilder BuildHeader(ResumeHeader header)
    {
        var headerTag = new TagBuilder("div");
        headerTag.AddCssClass("header");
        if(!string.IsNullOrEmpty(header.Name)) headerTag.InnerHtml.AppendHtml(_service.CreateSpan(header.Name, "name"));
        
        var details = new TagBuilder("div");
        details.AddCssClass("details");
        if (!string.IsNullOrEmpty(header.Email))
        {
            details.InnerHtml.AppendHtml(_service.CreateSpan(header.Email, "email"));
            details.InnerHtml.AppendHtml(_service.VerticalSeparator());
        }

        if (header.Phone != null)
        {
            details.InnerHtml.AppendHtml(_service.CreateSpan(header.Phone.FormattedNumber, "phone"));
            details.InnerHtml.AppendHtml(_service.VerticalSeparator());
        }
        if(!string.IsNullOrEmpty(header.Website)) details.InnerHtml.AppendHtml(_service.CreateSpan(header.Website, "website"));

        headerTag.InnerHtml.AppendHtml(details);
        return headerTag;
    }
}