namespace ResumeAPI.Models;

public class ResumeHeader
{
    public string Filename { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public PhoneNumber Phone { get; set; }
}

public class PhoneNumber
{
    public int AreaCode { get; set; }
    public int Prefix { get; set; }
    public int LineNumber { get; set; }
    public string FormattedNumber
    {
        get => $"({AreaCode}) {Prefix}-{LineNumber}";
    }
}