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
        converter.Options.PdfPageSize = PdfPageSize.Letter;
        
        var html = @"<style>
            .separator-container {
                display: flex;
                flex-direction: row;
                width: 100%;
            }
    
            .text {
                margin-left: 1rem;
                margin-right: 1rem;
                font-size: 3rem;
            }
        
            .separator{
                flex-grow: 1;
                margin: auto;
            }
        
            .separator hr {
                height: 2px;
                border-width: 0;
                background-color: black;
            }

            .page {
                width:1004px;
                height: 1304px;
                border: 2px solid black;
                margin-top: 18px;
            }

            #page1 {
                background-color: #ff0000;
                margin-top: 0;
            }

            #page2 {
                
                background-color: #00ff00;
            }
        </style>";
        
        // html += _service.BuildHeader(header);
        //
        // html += _service.BuildSummary(header);

        html += @"<div id=""page1""class=""page""></div><div id=""page2"" class=""page""></div>";
        
        PdfDocument doc = converter.ConvertHtmlString(html);
        doc.Save(stream);
        doc.Close();
        return stream;
    }
    
}