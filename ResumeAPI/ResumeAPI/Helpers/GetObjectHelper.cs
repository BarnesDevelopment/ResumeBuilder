using Microsoft.AspNetCore.Mvc;

namespace ResumeAPI.Helpers;

public static class GetObjectHelper
{
    public static T GetObject<T>(this ActionResult<T> actionResult)
    {
        return (T)((ObjectResult)actionResult.Result!).Value!;
    }
}