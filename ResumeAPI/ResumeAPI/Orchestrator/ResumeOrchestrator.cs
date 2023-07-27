using Microsoft.AspNetCore.Mvc.Rendering;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;
using SelectPdf;

namespace ResumeAPI.Orchestrator;

public interface IResumeOrchestrator
{
    MemoryStream BuildResume(ResumeHeader header);
    string BuildResumeHtml(ResumeHeader header);
}

public class ResumeOrchestrator : IResumeOrchestrator
{
    private readonly IResumeService _service;
    
    public ResumeOrchestrator(IResumeService service)
    {
        _service = service;
    }
    public MemoryStream BuildResume(ResumeHeader header)
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

    public string BuildResumeHtml(ResumeHeader header)
    {
        var html = new TagBuilder("html");
        html.InnerHtml.AppendHtml(CreateHtml(header));
        return(html.Write());
    }

    private TagBuilder CreateHtml(ResumeHeader header)
    {
        var page0 = _service.NewPage(0);
        page0.InnerHtml.AppendHtml(_service.BuildHeader(header));
        page0.InnerHtml.AppendHtml(_service.BuildSummary(header));

        var body = _service.BuildBody(new List<TagBuilder> { page0 });
        return body;
    }
}