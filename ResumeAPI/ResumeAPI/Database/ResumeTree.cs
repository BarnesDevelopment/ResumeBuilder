using Dapper;
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
        var query = $@"SELECT 
                        id {nameof(ResumeTreeNode.Id)},
                        userId {nameof(ResumeTreeNode.UserId)},
                        parentId {nameof(ResumeTreeNode.ParentId)},
                        content {nameof(ResumeTreeNode.Content)},
                        placementorder {nameof(ResumeTreeNode.Order)},
                        depth {nameof(ResumeTreeNode.Depth)},
                        sectiontype {nameof(ResumeTreeNode.SectionType)},
                        active {nameof(ResumeTreeNode.Active)}
                        FROM ResumeTree WHERE Id = @Id";
        return await Db.QuerySingleAsync<ResumeTreeNode>(query, new { Id = id });
    }

    public async Task<List<ResumeTreeNode>> GetChildren(Guid id)
    {
        var query = $@"SELECT 
                        id {nameof(ResumeTreeNode.Id)},
                        userId {nameof(ResumeTreeNode.UserId)},
                        parentId {nameof(ResumeTreeNode.ParentId)},
                        content {nameof(ResumeTreeNode.Content)},
                        placementorder {nameof(ResumeTreeNode.Order)},
                        depth {nameof(ResumeTreeNode.Depth)},
                        sectiontype {nameof(ResumeTreeNode.SectionType)},
                        active {nameof(ResumeTreeNode.Active)}
                        FROM ResumeTree WHERE parentid = @Id";
        return (await Db.QueryAsync<ResumeTreeNode>(query, new { Id = id })).ToList();
    }

    public async Task<List<ResumeTreeNode>> GetTopLevelNodes(Guid userId)
    {
        var query = $@"SELECT 
                        id {nameof(ResumeTreeNode.Id)},
                        userId {nameof(ResumeTreeNode.UserId)},
                        parentId {nameof(ResumeTreeNode.ParentId)},
                        content {nameof(ResumeTreeNode.Content)},
                        placementorder {nameof(ResumeTreeNode.Order)},
                        depth {nameof(ResumeTreeNode.Depth)},
                        sectiontype {nameof(ResumeTreeNode.SectionType)},
                        active {nameof(ResumeTreeNode.Active)}
                        FROM ResumeTree WHERE depth = 0";
        return (await Db.QueryAsync<ResumeTreeNode>(query)).ToList();
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