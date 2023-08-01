namespace ResumeAPI.Models;

public class User
{
    private Guid _id;
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public string Id
    {
        get => _id.ToString();
        set => _id = Guid.Parse(value);
    }
    public PasswordHash PasswordHash { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

public class UserViewModel
{
    private Guid _id;
    public string Id
    {
        get => _id.ToString();
        set => _id = Guid.Parse(value);
    }
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