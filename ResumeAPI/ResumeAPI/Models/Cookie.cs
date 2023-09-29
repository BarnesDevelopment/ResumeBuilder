#nullable disable
namespace ResumeAPI.Models;

public class Cookie
{
    public Guid Key { get; set; }
    public Guid UserId { get; set; }

    public DateTime Expiration { get; set; }
    public bool Active { get; set; }
}