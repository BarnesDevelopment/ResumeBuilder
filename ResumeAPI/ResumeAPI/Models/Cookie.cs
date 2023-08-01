#nullable disable
namespace ResumeAPI.Models;

public class Cookie
{
    private Guid _key;
    private Guid _userId;

    public Guid KeyGuid()
    {
        return _key;
    }

    public string Key
    {
        get => _key.ToString();
        set => _key = Guid.Parse(value);
    }
    
    public string UserId
    {
        get => _userId.ToString();
        set => _userId = Guid.Parse(value);
    }

    public DateTime Expiration { get; set; }
    public bool Active { get; set; }
}