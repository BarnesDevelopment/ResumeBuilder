namespace ResumeAPI.Models;

public class LoginAttempt
{
    public LoginAttempt()
    {
        Success = false;
    }

    public LoginAttempt(Cookie cookie)
    {
        Success = true;
        Cookie = cookie;
    }
    
    public bool Success { get; }
    public Cookie Cookie { get; }
}

public enum VerificationResult
{
    Correct,
    Incorrect,
    NotFound
}