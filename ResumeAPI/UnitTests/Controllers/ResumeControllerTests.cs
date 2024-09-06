using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ResumeAPI.Controllers;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;

namespace UnitTests.Controllers;

public class ResumeControllerTests
{
  private readonly ResumeController _controller;
  private readonly ILogger<ResumeController> _logger;
  private readonly IResumeOrchestrator _orchestrator;
  private readonly IUserValidator _userValidator;

  public ResumeControllerTests()
  {
    _orchestrator = Substitute.For<IResumeOrchestrator>();
    _logger = Substitute.For<ILogger<ResumeController>>();
    _userValidator = Substitute.For<IUserValidator>();
    _controller = new ResumeController(_logger, _orchestrator, _userValidator);
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

    _userValidator.Validate(Arg.Any<HttpContext>(), id).Returns(UserValidationResult.Valid);
    _orchestrator.GetResumeTree(id).Returns(expected);

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
    _userValidator.Validate(Arg.Any<HttpContext>(), id).Returns(UserValidationResult.NotFound);

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

    _userValidator.Validate(Arg.Any<HttpContext>(), id).Returns(UserValidationResult.Invalid);

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

    _userValidator.Validate(Arg.Any<HttpContext>(), id).Returns(UserValidationResult.Valid);
    _userValidator.GetUserId(Arg.Any<HttpContext>()).Returns(userId);
    _orchestrator.GetResumeTree(id).Throws(new Exception("some error"));

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

    _userValidator.ValidateUser(_controller.HttpContext).Returns(UserValidationResult.Valid);
    _userValidator.GetUserId(Arg.Any<HttpContext>()).Returns(userId);
    _orchestrator.GetTopLevelResumes(userId).Returns(expected);

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

    _userValidator.ValidateUser(_controller.HttpContext).Returns(UserValidationResult.Invalid);

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

    _userValidator.ValidateUser(_controller.HttpContext).Returns(UserValidationResult.Valid);
    _userValidator.GetUserId(Arg.Any<HttpContext>()).Returns(userId);
    _orchestrator.GetTopLevelResumes(userId).Throws(new Exception("some error"));

    var actual = await _controller.GetAllResumes();

    ((ObjectResult)actual.Result!).Value.Should().BeOfType<ProblemDetails>();
    ((ProblemDetails)((ObjectResult)actual.Result!).Value!).Detail.Should().Be("some error");
  }

  #endregion

  #region UpdateNode

  [Fact]
  public async Task UpdateNode_ReturnsNoContent()
  {
    var id = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var resume = new ResumeTreeNode { Id = id, UserId = userId };

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.ValidateUser(Arg.Any<HttpContext>()).Returns(UserValidationResult.Valid);
    _userValidator.GetUserId(Arg.Any<HttpContext>()).Returns(userId);
    _userValidator.ValidateResource(userId, id).Returns(UserValidationResult.Valid);
    _orchestrator.UpsertNode(resume, userId).Returns(resume);

    var actual = await _controller.UpsertNode(new[] { resume });

    actual.Should().BeOfType<NoContentResult>();
  }

  [Theory]
  [InlineData(UserValidationResult.Valid)]
  [InlineData(UserValidationResult.Invalid)]
  public async Task UpdateNode_InvalidUser_ReturnsForbidden(UserValidationResult result)
  {
    var id = Guid.NewGuid();
    var resume = new ResumeTreeNode
    {
      Id = id, UserId = Guid.NewGuid()
    };

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.ValidateUser(Arg.Any<HttpContext>()).Returns(result);
    _userValidator.ValidateResource(Arg.Any<Guid>(), id).Returns(UserValidationResult.Invalid);

    var actual = await _controller.UpsertNode(new[] { resume });

    actual.Should().BeOfType<ForbidResult>();
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

    _userValidator.ValidateUser(Arg.Any<HttpContext>()).Returns(UserValidationResult.Valid);
    _userValidator.GetUserId(Arg.Any<HttpContext>()).Returns(userId);
    _userValidator.ValidateResource(userId, id).Returns(UserValidationResult.Valid);
    _orchestrator.UpsertNode(resume, userId).Throws(new Exception("some error"));

    var actual = await _controller.UpsertNode(new[] { resume });

    ((ObjectResult)actual!).Value.Should().BeOfType<ProblemDetails>();
    ((ProblemDetails)((ObjectResult)actual!).Value!).Detail.Should().Be("some error");
  }

  #endregion

  #region DeleteNode

  [Theory]
  [InlineData(UserValidationResult.Valid)]
  [InlineData(UserValidationResult.NotFound)]
  public async Task DeleteNode_ReturnsAccepted(UserValidationResult result)
  {
    var id = Guid.NewGuid();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Validate(Arg.Any<HttpContext>(), id).Returns(result);
    _orchestrator.DeleteNode(id).Returns(true);

    var actual = await _controller.DeleteNode(id);

    actual.Should().BeOfType<NoContentResult>();
  }

  [Fact]
  public async Task DeleteNode_ReturnsForbidden()
  {
    var id = Guid.NewGuid();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext()
    };

    _userValidator.Validate(Arg.Any<HttpContext>(), id).Returns(UserValidationResult.Invalid);

    var actual = await _controller.DeleteNode(id);

    actual.Should().BeOfType<ForbidResult>();
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

    _userValidator.Validate(Arg.Any<HttpContext>(), id).Returns(UserValidationResult.Valid);
    _userValidator.GetUserId(Arg.Any<HttpContext>()).Returns(userId);
    _orchestrator.DeleteNode(id).Throws(new Exception("some error"));

    var actual = await _controller.DeleteNode(id);

    ((ObjectResult)actual).Value.Should().BeOfType<ProblemDetails>();
    ((ProblemDetails)((ObjectResult)actual).Value!).Detail.Should().Be("some error");
  }

  #endregion
}
