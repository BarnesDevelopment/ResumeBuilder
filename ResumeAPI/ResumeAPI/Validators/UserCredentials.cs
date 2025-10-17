using FluentValidation;
using ResumeAPI.Services;

namespace ResumeAPI.Validators;

public class UserCredentials : AbstractValidator<HttpContext>
{
    public UserCredentials(IAuthClient authClient)
    {
        RuleFor(context => context.Request)
            .SetValidator(new RequestValidator(authClient));
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
            .WithMessage("Missing Authorization header.")
            .Must(headers => !string.IsNullOrEmpty(headers["Authorization"]))
            .WithMessage("Authorization header cannot be empty.")
            .Must(headers => headers["Authorization"].ToString().StartsWith("Bearer "))
            .WithMessage("Authorization header must start with 'Bearer '.")
            .MustAsync(async (headers, _) =>
            {
                var token = headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
                
                var response = await authClient.AuthenticateJwt(token);

                return response;
            })
            .WithMessage("Invalid Authorization header.");
    }
}