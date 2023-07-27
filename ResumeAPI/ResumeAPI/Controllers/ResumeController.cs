using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;

namespace ResumeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResumeController : ControllerBase
    {
        private readonly ILogger<ResumeController> _logger;
        private readonly IResumeOrchestrator _orchestrator;

        public ResumeController(ILogger<ResumeController> logger, IResumeOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
            _logger = logger;
        }

        [HttpPost("/build")]
        public IActionResult BuildPdf([FromBody] ResumeHeader header)
        {
            var stream = _orchestrator.BuildResume(header);

            return File(stream.GetBuffer(), "application/octet-stream", header.Filename);
        }

        [HttpGet("/build-test")]
        public IActionResult BuildPdfTest()
        {
            var header = new ResumeHeader
            {
                Filename = "test.pdf",
                Name = "test",
                Email = "test@test.com",
                Website = "test.com",
                Phone = new PhoneNumber
                {
                    AreaCode = 555,
                    Prefix = 867,
                    LineNumber = 5309
                },
                Summary = "some stupid summary"
            };

            return Content(_orchestrator.BuildResumeHtml(header), "text/html");
        }
    }
}