namespace ResumeAPI.Models;

public class AppSettings
{
    public Jwt Jwt { get; set; }
    public string InfisicalClientId { get; set; }
    public string InfisicalClientSecret { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; }
}

public class Jwt
{
    public string Authority { get; set; }
}

public class ConnectionStrings
{
    public string Postgres { get; set; }
}