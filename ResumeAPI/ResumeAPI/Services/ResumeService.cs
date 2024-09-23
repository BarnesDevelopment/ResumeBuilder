using ResumeAPI.Database;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IResumeService
{
  Task<Guid> DuplicateResume(ResumeTreeNode resume);
  Task<ResumeTreeNode> GetFullResumeTree(ResumeTreeNode root);
}

public class ResumeService : IResumeService
{
  private readonly IResumeTree _tree;

  public ResumeService(IResumeTree tree)
  {
    _tree = tree;
  }

  public async Task<Guid> DuplicateResume(ResumeTreeNode resume)
  {
    return Guid.Empty;
  }

  public async Task<ResumeTreeNode> GetFullResumeTree(ResumeTreeNode root)
  {
    root.Children = await _tree.GetChildren(root.Id);

    return await GetGrandChildren(root);
  }

  private async Task<ResumeTreeNode> GetGrandChildren(ResumeTreeNode root)
  {
    for (var i = 0; i < root.Children.Count; i++)
    {
      root.Children[i].Children = await _tree.GetChildren(root.Children[i].Id);
      root.Children[i] = await GetGrandChildren(root.Children[i]);
    }

    return root;
  }
}
