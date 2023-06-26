using ResumeAPI.Models;
using ResumeAPI.Services;
using SelectPdf;

namespace ResumeAPI.Orchestrator;

public interface IResumeOrchestrator
{
    MemoryStream BuildResume(ResumeHeader header);
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
        
        var html = "<p>Hello World</p><h1>Hi suckers!</h1>";
        
        html += _service.BuildHeader(header);
        
        html += _service.BuildSummary(header);
        
        PdfDocument doc = converter.ConvertHtmlString(html);
        doc.Save(stream);
        doc.Close();
        return stream;
    }
    
}