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
  Task<ResumeTreeNode> UpsertNode(ResumeTreeNode node);
  Task<bool> DeleteNode(Guid id);
}

public class ResumeTree : PostgreSqlContext, IResumeTree
{
  public ResumeTree(IOptions<AWSSecrets> options) : base(options)
  {
  }

  public async Task<ResumeTreeNode?> GetNode(Guid id)
  {
    var query = $@"SELECT 
                        id {nameof(ResumeTreeNode.Id)},
                        userId {nameof(ResumeTreeNode.UserId)},
                        parentId {nameof(ResumeTreeNode.ParentId)},
                        content {nameof(ResumeTreeNode.Content)},
                        placementorder as {nameof(ResumeTreeNode.Order)},
                        depth {nameof(ResumeTreeNode.Depth)},
                        sectiontype {nameof(ResumeTreeNode.NodeType)},
                        active {nameof(ResumeTreeNode.Active)},
                        comments {nameof(ResumeTreeNode.Comments)}
                        FROM ResumeDb.ResumeTree WHERE Id = @Id";
    return (await Db.QueryAsync<ResumeTreeNode>(query, new { Id = id })).FirstOrDefault();
  }

  public async Task<List<ResumeTreeNode>> GetChildren(Guid id)
  {
    var query = $@"SELECT 
                        id {nameof(ResumeTreeNode.Id)},
                        userId {nameof(ResumeTreeNode.UserId)},
                        parentId {nameof(ResumeTreeNode.ParentId)},
                        content {nameof(ResumeTreeNode.Content)},
                        placementorder as {nameof(ResumeTreeNode.Order)},
                        depth {nameof(ResumeTreeNode.Depth)},
                        sectiontype {nameof(ResumeTreeNode.NodeType)},
                        active {nameof(ResumeTreeNode.Active)},
                        comments {nameof(ResumeTreeNode.Comments)}
                        FROM ResumeDb.ResumeTree WHERE parentid = @Id
                        order by placementorder";
    return (await Db.QueryAsync<ResumeTreeNode>(query, new { Id = id })).ToList();
  }

  public async Task<List<ResumeTreeNode>> GetTopLevelNodes(Guid userId)
  {
    var query = $@"SELECT 
                        id {nameof(ResumeTreeNode.Id)},
                        userId {nameof(ResumeTreeNode.UserId)},
                        parentId {nameof(ResumeTreeNode.ParentId)},
                        content {nameof(ResumeTreeNode.Content)},
                        placementorder as {nameof(ResumeTreeNode.Order)},
                        depth {nameof(ResumeTreeNode.Depth)},
                        sectiontype {nameof(ResumeTreeNode.NodeType)},
                        active {nameof(ResumeTreeNode.Active)},
                        comments {nameof(ResumeTreeNode.Comments)}
                        FROM ResumeDb.ResumeTree WHERE depth = 0 and id = :userId";
    return (await Db.QueryAsync<ResumeTreeNode>(query, new { userId })).ToList();
  }

  public async Task<bool> CreateNode(ResumeTreeNode node)
  {
    var createQuery = $@"INSERT INTO ResumeDb.ResumeTree 
                            (id, userId, parentId, content, placementorder, depth, sectiontype, active, comments) 
                            VALUES (@Id, @UserId, @ParentId, @Content, @Order, @Depth, @NodeType, @Active, @Comments)";
    return await Db.ExecuteAsync(createQuery, node) > 0;
  }

  public async Task<ResumeTreeNode> UpsertNode(ResumeTreeNode node)
  {
    var updateQuery = $@"INSERT INTO ResumeDb.ResumeTree
                            (id, userId, parentId, content, placementorder, depth, sectiontype, active, comments)
                            VALUES (@Id, @UserId, @ParentId, @Content, @Order, @Depth, @NodeType, @Active, @Comments)
                            ON CONFLICT (id) DO UPDATE SET 
                            userId = @UserId, 
                            parentId = @ParentId, 
                            content = @Content, 
                            placementorder = @Order,
                            depth = @Depth, 
                            sectiontype = @NodeType, 
                            active = @Active,
                            comments = @Comments";
    await Db.ExecuteAsync(updateQuery, node);
    var query = $@"SELECT 
                        id {nameof(ResumeTreeNode.Id)},
                        userId {nameof(ResumeTreeNode.UserId)},
                        parentId {nameof(ResumeTreeNode.ParentId)},
                        content {nameof(ResumeTreeNode.Content)},
                        placementorder as {nameof(ResumeTreeNode.Order)},
                        depth {nameof(ResumeTreeNode.Depth)},
                        sectiontype {nameof(ResumeTreeNode.NodeType)},
                        active {nameof(ResumeTreeNode.Active)},
                        comments {nameof(ResumeTreeNode.Comments)}
                        FROM ResumeDb.ResumeTree WHERE id = @Id";
    return await Db.QuerySingleAsync<ResumeTreeNode>(query, new { Id = node.Id });
  }

  public async Task<bool> DeleteNode(Guid id)
  {
    var query = "DELETE FROM ResumeDb.ResumeTree WHERE id = @Id";
    return await Db.ExecuteAsync(query, new { Id = id }) > 0;
  }
}
