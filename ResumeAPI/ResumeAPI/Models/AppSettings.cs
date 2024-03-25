namespace ResumeAPI.Models;

public class AppSettings
{
  public Jwt Jwt { get; set; }
}

public class Jwt
{
  public string Authority { get; set; }
  public string Audience { get; set; }
}
