namespace ResumeAPI.Models;

public class User
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Guid Id { get; set; }
    public byte[] Salt { get; set; }
    public PasswordHash PasswordHash { get; set; }
}

public class UserViewModel
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get => $"{FirstName} {LastName}"; }
    public string Email { get; set; }
}

public class PasswordHash
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Hash { get; set; }
}