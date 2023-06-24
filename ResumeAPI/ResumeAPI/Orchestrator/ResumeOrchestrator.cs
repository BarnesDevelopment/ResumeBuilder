using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using ResumeAPI.Models;
using ResumeAPI.Services;

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
        Document document = new Document();

        var page = _service.AddPage(document);

        _service.BuildHeader(page,header);

        document.Draw(stream);
        return stream;
    }
    
}