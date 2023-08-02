namespace ResumeAPI.Helpers;

public class AWSSecrets
{
    public AWSSecrets()
    {
        ConnectionStrings = new ConnectionStrings();
    }
    
    public string ConnectionStrings_MySql { set => ConnectionStrings.MySql = value; }
    public ConnectionStrings ConnectionStrings { get; set; }
}

public class ConnectionStrings
{
    public string MySql { get; set; }
}