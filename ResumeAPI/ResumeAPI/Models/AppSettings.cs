namespace ResumeAPI.Models;

public class AppSettings
{
    public required Jwt Jwt { get; set; }
    public required string InfisicalClientId { get; set; }
    public required string InfisicalClientSecret { get; set; }
    public required ConnectionStrings ConnectionStrings { get; set; }
}

public class Jwt
{
    public required string Authority { get; set; }
}

public class ConnectionStrings
{
    public required string Postgres { get; set; }
}