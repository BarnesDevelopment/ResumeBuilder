using Dapper;
using Microsoft.Extensions.Options;
using ResumeAPI.Models;

namespace ResumeAPI.Database;

public interface IResumeTree
{
    Task<ResumeTreeNode?> GetNode(Guid id);
    Task<List<ResumeTreeNode>> GetChildren(Guid id);
    Task<List<ResumeTreeNode>> GetTopLevelNodes(Guid userId);
    Task<bool> CreateNode(ResumeTreeNode node);
    Task<ResumeTreeNode> UpdateNode(ResumeTreeNode node);
    Task<bool> DeleteNode(Guid id);
}

public class ResumeTree : PostgreSqlContext, IResumeTree
{
    public ResumeTree(IOptions<AWSSecrets> options) : base(options) {}

    public async Task<ResumeTreeNode?> GetNode(Guid id)
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

    public async Task<bool> CreateNode(ResumeTreeNode node)
    {
        var createQuery = $@"INSERT INTO ResumeTree 
                            (id, userId, parentId, content, placementorder, depth, sectiontype, active) 
                            VALUES (@Id, @UserId, @ParentId, @Content, @Order, @Depth, @SectionType, @Active)";
        return await Db.ExecuteAsync(createQuery, node) > 0;
    }

    public async Task<ResumeTreeNode> UpdateNode(ResumeTreeNode node)
    {
        var updateQuery = $@"UPDATE ResumeTree SET 
                            userId = @UserId, 
                            parentId = @ParentId, 
                            content = @Content, 
                            placementorder = @Order, 
                            depth = @Depth, 
                            sectiontype = @SectionType, 
                            active = @Active 
                            WHERE id = @Id";
        await Db.ExecuteAsync(updateQuery, node);
        var query = $@"SELECT 
                        id {nameof(ResumeTreeNode.Id)},
                        userId {nameof(ResumeTreeNode.UserId)},
                        parentId {nameof(ResumeTreeNode.ParentId)},
                        content {nameof(ResumeTreeNode.Content)},
                        placementorder {nameof(ResumeTreeNode.Order)},
                        depth {nameof(ResumeTreeNode.Depth)},
                        sectiontype {nameof(ResumeTreeNode.SectionType)},
                        active {nameof(ResumeTreeNode.Active)}
                        FROM ResumeTree WHERE id = @Id";
        return await Db.QuerySingleAsync<ResumeTreeNode>(query, new { Id = node.Id });
    }

    public async Task<bool> DeleteNode(Guid id)
    {
        var query = "DELETE FROM ResumeTree WHERE id = @Id";
        return (await Db.ExecuteAsync(query, new { Id = id })) > 0;
    }
}