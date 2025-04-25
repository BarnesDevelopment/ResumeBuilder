#nullable disable
namespace ResumeAPI.Models;

public class User
{
    public bool Demo;
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Cookie { get; set; }
    public DateTime CookieExpiration { get; set; }
}