using FluentValidation;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Services;

namespace ResumeAPI.Validators;

public class UserCredentials : AbstractValidator<(HttpContext httpContext, Guid? resourceId)>
{
    public UserCredentials(IResumeTree resumeDb, IAuthClient authClient)
    {
        ResumeTreeNode? resource = null;
        RuleFor(context => context.httpContext.Request)
            .SetValidator(new RequestValidator(authClient))
            .DependentRules(() => RuleFor(context => context.resourceId)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (context, resourceId, _) =>
                {
                    if (resourceId == null) return true;

                    resource = await resumeDb.GetNode(resourceId.Value);
                    return resource != null;
                })
                .WithMessage("The specified resource does not exist. 404")
                .Must((context, resourceId, _) =>
                {
                    if (resourceId == null) return true;

                    var userId = context.httpContext.GetUserId();

                    return resource!.UserId == userId;
                })
                .WithMessage("User does not have access to the specified resource. 403"));
    }
}

public class RequestValidator : AbstractValidator<HttpRequest>
{
    public RequestValidator(IAuthClient authClient)
    {
        RuleFor(request => request.Headers)
            .SetValidator(new HeaderValidator(authClient));
    }
}

public class HeaderValidator : AbstractValidator<IHeaderDictionary>
{
    public HeaderValidator(IAuthClient authClient)
    {
        RuleFor(headers => headers)
            .Cascade(CascadeMode.Stop)
            .Must(headers => headers.ContainsKey("Authorization"))
            .WithMessage("Missing Authorization header. 401")
            .Must(headers => !string.IsNullOrEmpty(headers["Authorization"]))
            .WithMessage("Authorization header cannot be empty. 401")
            .Must(headers => headers["Authorization"].ToString().StartsWith("Bearer "))
            .WithMessage("Authorization header must start with 'Bearer '. 401")
            .MustAsync(async (headers, _) =>
            {
                var token = headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();

                var response = await authClient.AuthenticateJwt(token);

                return response;
            })
            .WithMessage("Invalid Authorization header. 401");
    }
}