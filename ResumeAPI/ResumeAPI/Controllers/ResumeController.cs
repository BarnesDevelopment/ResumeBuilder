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
        public IActionResult BuildPdf([FromBody] Resume resume)
        {
            var stream = _orchestrator.BuildResume(resume);

            return File(stream.GetBuffer(), "application/octet-stream", resume.Header.Filename);
        }

        [HttpGet("/build-test")]
        public IActionResult BuildPdfTest()
        {
            var resume = new Resume
            {
                Header = new ResumeHeader
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
                    Summary =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque quis tortor magna. Aliquam erat volutpat. Nullam maximus diam a ante tempus, eget rhoncus enim iaculis. Suspendisse quis nibh in urna feugiat faucibus a id nunc. Phasellus sed ex metus. Morbi iaculis nunc non odio lobortis porta. Duis porttitor, diam eget tincidunt tempus, urna enim sollicitudin mauris, sed posuere ante sem nec arcu. Mauris et facilisis sem, et ornare ex. Maecenas a euismod ipsum. Nunc sodales, leo non maximus commodo, ex augue lobortis erat, quis malesuada ligula urna sed libero. Duis luctus suscipit purus. Donec ultrices tellus augue, ut porta tortor feugiat ac. Vivamus cursus fermentum accumsan. Vestibulum quam ligula, sodales a justo eu, blandit eleifend metus. Nunc semper imperdiet libero at tincidunt. Maecenas lacinia posuere viverra."
                },
                Education = new List<ResumeEducation>
                {
                    new()
                    {
                        School = "William Penn University",
                        City = "Oskaloosa",
                        State = "IA",
                        GraduationYear = "2020",
                        Degree = new()
                        {
                            TypeOfDegree = "Bachelor of Arts",
                            Major = "Software Engineering & Information Technology",
                            Minor = "Electrical Engineering"
                        }
                    },
                    new()
                    {
                        School = "Barnes Academy",
                        City = "Pella",
                        State = "IA",
                        GraduationYear = "2016"
                    }
                },
                Experience = new List<ResumeExperience>
                {
                    new()
                    {
                        JobTitle = "Software Engineer",
                        Employer = "Vermeer Corporation",
                        City = "Pella",
                        State = "IA",
                        StartDate = new DateTime(2021,03,01),
                        EndDate = null,
                        Responsibilities = new List<string>
                        {
                            "Develop software for dealer facing applications",
                            "Manage and track time and work completed",
                            "Complete tasks under time pressure",
                            "Multitask working and training other team members"
                        }
                    },
                    new()
                    {
                        JobTitle = "Welder 2nd Shift",
                        Employer = "Vermeer",
                        City = "Pella",
                        State = "IA",
                        StartDate = new DateTime(2019,05,01),
                        EndDate = new DateTime(2021,03,01),
                        Responsibilities = new List<string>
                        {
                            "Weld parts to print",
                            "Fill in for welders that are missing",
                            "Clean area at the end of every shift"
                        }
                    }
                },
                Skills = new List<string>
                {
                    "Proficient in computing",
                    "Excellent customer service skills",
                    "Responsible and punctual",
                    "ServeSafe certified",
                    "Coding experience",
                    "Client-focused",
                    "Quick learner"
                }
            };

            return Content(_orchestrator.BuildResumeHtml(resume), "text/html");
        }
    }
}