#nullable disable
namespace ResumeAPI.Models;

public class User : UserViewModel
{
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}

public class UserViewModel : UserInfo
{
    public UserViewModel(UserInfo info)
    {
        Username = info.Username;
        FirstName = info.FirstName;
        LastName = info.LastName;
        Email = info.Email;
    }

    public UserViewModel()
    {
        
    }
    
    public Guid Id { get; set; }

}

public class UserInfo
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get => $"{FirstName} {LastName}"; }
    public string Email { get; set; }
}