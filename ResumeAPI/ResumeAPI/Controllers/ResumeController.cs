using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using Microsoft.AspNetCore.Mvc;
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
    }
}