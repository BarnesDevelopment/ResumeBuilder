using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ResumeAPI.Controllers;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;

namespace UnitTests.Controllers;

public class ResumeControllerTests
{
  private readonly ResumeController _controller;
  private readonly Mock<IResumeOrchestrator> _orchestrator;
  private readonly Mock<ILogger<ResumeController>> _logger;
  private readonly Mock<IUserValidator> _userValidator;

  public ResumeControllerTests()
  {
    _orchestrator = new Mock<IResumeOrchestrator>();
    _logger = new Mock<ILogger<ResumeController>>();
    _userValidator = new Mock<IUserValidator>();
    _controller = new ResumeController(_logger.Object, _orchestrator.Object, _userValidator.Object);
  }

  #region GetResumeById

  [Fact]
  public async Task GetResumeById_ShouldReturnResume()
  {
    var id = Guid.NewGuid();
    var expected = new ResumeTreeNode { Id = id };
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Valid);
    _orchestrator.Setup(x => x.GetResumeTree(id)).ReturnsAsync(expected);

    var actual = (await _controller.GetResumeById(id)).GetObject();

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task GetResumeById_ShouldReturnNotFound()
  {
    var id = Guid.NewGuid();
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };
    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.NotFound);

    var actual = await _controller.GetResumeById(id);

    actual.Result.Should().BeOfType<NotFoundObjectResult>();
    ((NotFoundObjectResult)actual.Result!).Value.Should().Be("Resource not found");
  }

  [Fact]
  public async Task GetResumeById_ShouldReturnUnauthorized()
  {
    var id = Guid.NewGuid();
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Invalid);

    var actual = await _controller.GetResumeById(id);

    actual.Result.Should().BeOfType<UnauthorizedResult>();
  }

  [Fact]
  public async Task GetResumeById_ShouldReturnProblem()
  {
    var id = Guid.NewGuid();
    var userId = Guid.NewGuid();
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Valid);
    _userValidator.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(userId);
    _orchestrator.Setup(x => x.GetResumeTree(id)).ThrowsAsync(new Exception("some error"));

    var actual = await _controller.GetResumeById(id);

    ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
    ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
  }

  #endregion

  #region GetAllResumes

  [Fact]
  public async Task GetAllResumes_ShouldReturnResumes()
  {
    var userId = Guid.NewGuid();
    var expected = new List<ResumeTreeNode> { new() };
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.ValidateUser(_controller.HttpContext)).ReturnsAsync(UserValidationResult.Valid);
    _userValidator.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(userId);
    _orchestrator.Setup(x => x.GetTopLevelResumes(userId)).ReturnsAsync(expected);

    var actual = (await _controller.GetAllResumes()).GetObject();

    actual.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task GetAllResumes_ShouldReturnUnauthorized()
  {
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.ValidateUser(_controller.HttpContext)).ReturnsAsync(UserValidationResult.Invalid);

    var actual = await _controller.GetAllResumes();

    actual.Result.Should().BeOfType<UnauthorizedResult>();
  }

  [Fact]
  public async Task GetAllResumes_ShouldReturnProblem()
  {
    var userId = Guid.NewGuid();
    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.ValidateUser(_controller.HttpContext)).ReturnsAsync(UserValidationResult.Valid);
    _userValidator.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(userId);
    _orchestrator.Setup(x => x.GetTopLevelResumes(userId)).ThrowsAsync(new Exception("some error"));

    var actual = await _controller.GetAllResumes();

    ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
    ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
  }

  #endregion

  #region UpdateNode

  [Fact]
  public async Task UpdateNode_ReturnsUpdatedResume()
  {
    var id = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var resume = new ResumeTreeNode { Id = id };

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Valid);
    _userValidator.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(userId);
    _orchestrator.Setup(x => x.UpsertNode(resume, userId)).ReturnsAsync(resume);

    var actual = (await _controller.UpsertNode(resume)).GetObject();

    actual.Should().BeEquivalentTo(resume);
  }

  [Fact]
  public async Task UpdateNode_ReturnsUnauthorized()
  {
    var id = Guid.NewGuid();
    var resume = new ResumeTreeNode { Id = id };

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Invalid);

    var actual = await _controller.UpsertNode(resume);

    actual.Result.Should().BeOfType<UnauthorizedResult>();
  }

  [Fact]
  public async Task UpdateNode_ReturnsProblem()
  {
    var id = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var resume = new ResumeTreeNode { Id = id };

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Valid);
    _userValidator.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(userId);
    _orchestrator.Setup(x => x.UpsertNode(resume, userId)).ThrowsAsync(new Exception("some error"));

    var actual = await _controller.UpsertNode(resume);

    ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
    ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
  }

  #endregion

  #region DeleteNode

  [Fact]
  public async Task DeleteNode_ReturnsAccepted()
  {
    var id = Guid.NewGuid();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Valid);
    _orchestrator.Setup(x => x.DeleteNode(id)).ReturnsAsync(true);

    var actual = await _controller.DeleteNode(id);

    actual.Should().BeOfType<AcceptedResult>();
    ((AcceptedResult)actual).Value.Should().Be(true);
  }

  [Fact]
  public async Task DeleteNode_ReturnsNotFound()
  {
    var id = Guid.NewGuid();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Valid);
    _orchestrator.Setup(x => x.DeleteNode(id)).ReturnsAsync(false);

    var actual = await _controller.DeleteNode(id);

    actual.Should().BeOfType<AcceptedResult>();
    ((AcceptedResult)actual).Value.Should().Be(false);
  }

  [Fact]
  public async Task DeleteNode_ReturnsUnauthorized()
  {
    var id = Guid.NewGuid();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Invalid);

    var actual = await _controller.DeleteNode(id);

    actual.Should().BeOfType<UnauthorizedResult>();
  }

  [Fact]
  public async Task DeleteNode_ReturnsProblem()
  {
    var id = Guid.NewGuid();
    var userId = Guid.NewGuid();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Setup(x => x.Validate(It.IsAny<HttpContext>(), id)).ReturnsAsync(UserValidationResult.Valid);
    _userValidator.Setup(x => x.GetUserId(It.IsAny<HttpContext>())).Returns(userId);
    _orchestrator.Setup(x => x.DeleteNode(id)).ThrowsAsync(new Exception("some error"));

    var actual = await _controller.DeleteNode(id);

    ((ObjectResult)actual).Value.Should().BeOfType<ProblemDetails>();
    ((ProblemDetails)((ObjectResult)actual).Value!).Detail.Should().Be("some error");
  }

  #endregion
}
