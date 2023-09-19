using Microsoft.Extensions.Options;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public interface IResumeData
{
}

public class ResumeData : MySqlContext, IResumeData
{
    public ResumeData(IOptions<AWSSecrets> options) : base(options) {}
    
}