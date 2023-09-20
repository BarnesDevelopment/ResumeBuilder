using Microsoft.AspNetCore.Mvc;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;

namespace ResumeAPI.Controllers
{
    [ApiController]
    [Route("resume/")]
    public class ResumeController : ControllerBase
    {
        private readonly ILogger<ResumeController> _logger;
        private readonly IResumeOrchestrator _orchestrator;

        public ResumeController(ILogger<ResumeController> logger, IResumeOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
            _logger = logger;
        }

        [HttpPost("build")]
        public IActionResult BuildPdf([FromBody] Resume resume)
        {
            var stream = _orchestrator.BuildResume(resume);

            return File(stream.GetBuffer(), "application/octet-stream", resume.Header.Filename);
        }

        #region Testing
        
        [HttpGet("build-test")]
        public IActionResult BuildPdfTest()
        {
            var resume = new Resume
            {
                SplitResume = true,
                SplitExperienceAfter = 4,
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
                        StartDate = new(2021,03,01, 0,0,0, DateTimeKind.Utc),
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
                        Employer = "Vermeer Corporation",
                        City = "Pella",
                        State = "IA",
                        StartDate = new DateTime(2019,05,01, 0,0,0, DateTimeKind.Utc),
                        EndDate = new DateTime(2021,03,01, 0,0,0, DateTimeKind.Utc),
                        Responsibilities = new List<string>
                        {
                            "Weld parts to print",
                            "Fill in for welders that are missing",
                            "Clean area at the end of every shift"
                        }
                    },
                    new()
                    {
                        JobTitle = "Customer Experience Expediter 1",
                        Employer = "Vermeer Corporation",
                        City = "Pella",
                        State = "IA",
                        StartDate = new DateTime(2018,05,01, 0,0,0, DateTimeKind.Utc),
                        EndDate = new DateTime(2019,04,01, 0,0,0, DateTimeKind.Utc),
                        Responsibilities = new List<string>
                        {
                            "Pick and pack parts",
                            "Prepare and fill out shipping paperwork",
                            "Operate machinery",
                            "Help other team members when necessary",
                            "Clean when necessary"
                        }
                    },
                    new()
                    {
                        JobTitle = "Delivery Driver",
                        Employer = "Pizza Ranch",
                        City = "Oskaloosa",
                        State = "IA",
                        StartDate = new DateTime(2017,12,01, 0,0,0, DateTimeKind.Utc),
                        EndDate = new DateTime(2018,02,01, 0,0,0, DateTimeKind.Utc),
                        Responsibilities = new List<string>
                        {
                            "Deliver orders",
                            "Wash dishes",
                            "Sweep/vacuum"
                        }
                    },
                    new()
                    {
                        JobTitle = "Shift Manager",
                        Employer = "Arby's",
                        City = "Pella",
                        State = "IA",
                        StartDate = new DateTime(2015,07,01, 0,0,0, DateTimeKind.Utc),
                        EndDate = new DateTime(2017,05,01, 0,0,0, DateTimeKind.Utc),
                        Responsibilities = new List<string>
                        {
                            "Manage employees",
                            "Operate register",
                            "Prepare food",
                            "Train team members",
                            "Clean",
                            "Take inventory",
                            "Manage store operations",
                            "Manage customer complaints"
                        }
                    },
                    new()
                    {
                        JobTitle = "Crew",
                        Employer = "Culvers",
                        City = "Pella",
                        State = "IA",
                        StartDate = new DateTime(2016,11,01, 0,0,0, DateTimeKind.Utc),
                        EndDate = new DateTime(2017,05,01, 0,0,0, DateTimeKind.Utc),
                        Responsibilities = new List<string>
                        {
                            "Grilled burgers",
                            "Build burgers",
                            "Clean",
                            "Operate fryers"

                        }
                    },
                    new()
                    {
                        JobTitle = "Manger",
                        Employer = "Mango Tree",
                        City = "Oskaloosa",
                        State = "IA",
                        StartDate = new DateTime(2016,02,01, 0,0,0, DateTimeKind.Utc),
                        EndDate = new DateTime(2021,11,01, 0,0,0, DateTimeKind.Utc),
                        Responsibilities = new List<string>
                        {
                            "Manage customer complaints",
                            "Manage volunteers",
                            "Clean",
                            "Operate register",
                            "Buy product for the store"
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
        
        #endregion
        
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<ResumeTreeNode>>> GetAllResumes()
        {
            try
            {
                var cookie = Request.Headers.Authorization.ToString();
                if (string.IsNullOrEmpty(cookie)) return Unauthorized();
                return Ok(await _orchestrator.GetTopLevelResumes(cookie));
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return Problem(e.Message);
            }
        }
        
        [HttpGet("get/{id:guid}")]
        public async Task<ActionResult<ResumeTreeNode>> GetResumeById(Guid id)
        {
            try
            {
                var cookie = Request.Headers.Authorization.ToString();
                if (string.IsNullOrEmpty(cookie)) return Unauthorized();
                var resume = await _orchestrator.GetResumeTree(id, cookie);
                if (resume != null) return Ok(resume);
                return NotFound();
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return Problem(e.Message);
            }
        }
        
        [HttpPost("create")]
        public async Task<ActionResult<ResumeTreeNode>> CreateResume([FromBody] ResumeTreeNode resume)
        {
            try
            {  
                var cookie = Request.Headers.Authorization.ToString();
                if (string.IsNullOrEmpty(cookie)) return Unauthorized();
                return Ok(await _orchestrator.CreateResume(resume, cookie));
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return Problem(e.Message);
            }
        }
        
        [HttpPost("update/{id:guid}")]
        public async Task<ActionResult<ResumeTreeNode>> UpdateNode(Guid id, [FromBody] ResumeTreeNode resume)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<ActionResult> DeleteNode(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}