using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using Microsoft.AspNetCore.Mvc;

namespace ResumeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly ILogger<ResumeController> _logger;

        public ResumeController(ILogger<ResumeController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/build")]
        public IActionResult BuildPdf()
        {
            var stream = new MemoryStream();
            // processing the stream.

            Document document = new Document();
            
            Page page = new Page(PageSize.Letter, PageOrientation.Portrait, 54.0f);
            document.Pages.Add(page);

            string labelText = "Hello World...\nFrom DynamicPDF Generator for .NET\nDynamicPDF.com";
            Label label = new Label(labelText, 0, 0, 504, 100, Font.Helvetica, 18, TextAlign.Center);
            page.Elements.Add(label);

            document.Draw(stream);

            var fileName = "file.pdf";

            return File(stream.GetBuffer(), "application/octet-stream", fileName);
        }
    }
}