using Microsoft.Extensions.Options;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public interface IResumeTree
{
    Task<ResumeTreeNode> GetNode(Guid id);
    Task<List<ResumeTreeNode>> GetChildren(Guid id);
    Task<List<ResumeTreeNode>> GetTopLevelNodes(Guid userId);
    Task<ResumeTreeNode> CreateNode(ResumeTreeNode node);
    Task<ResumeTreeNode> UpdateNode(ResumeTreeNode node);
    Task<ResumeTreeNode> DeleteNode(Guid id);
}

public class ResumeTree : MySqlContext, IResumeTree
{
    public ResumeTree(IOptions<AWSSecrets> options) : base(options) {}

    public async Task<ResumeTreeNode> GetNode(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ResumeTreeNode>> GetChildren(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ResumeTreeNode>> GetTopLevelNodes(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ResumeTreeNode> CreateNode(ResumeTreeNode node)
    {
        throw new NotImplementedException();
    }

    public async Task<ResumeTreeNode> UpdateNode(ResumeTreeNode node)
    {
        throw new NotImplementedException();
    }

    public async Task<ResumeTreeNode> DeleteNode(Guid id)
    {
        throw new NotImplementedException();
    }
}