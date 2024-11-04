using Microsoft.AspNetCore.Http;

namespace UnitTests.Models;

public class RequestCookieCollection : Dictionary<string, string>, IRequestCookieCollection
{
    public new ICollection<string> Keys => base.Keys;

    public new string? this[string key]
    {
        get
        {
            TryGetValue(key, out var value);
            return value;
        }
        set
        {
            if (value != null) base[key] = value;
        }
    }
}