#nullable disable

namespace ResumeAPI.Models;

public class Resume
{
    public ResumeHeader Header { get; set; }
    public List<ResumeEducation> Education { get; set; }
    public List<ResumeExperience> Experience { get; set; }
    public List<string> Skills { get; set; }
}

public class ResumeExperience
{
    public string JobTitle { get; set; }
    public string Employer { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string> Responsibilities { get; set; }
}

public class ResumeEducation
{
    public string School { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string GraduationYear { get; set; }
    public ResumeDegree Degree { get; set; } 
}

public class ResumeDegree
{
    public string TypeOfDegree { get; set; }
    public string Major { get; set; }
    public string Minor { get; set; }
}

public class ResumeHeader
{
    public string Filename { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public PhoneNumber Phone { get; set; }
    public string Summary { get; set; }
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