using ResumeAPI.Database;
using ResumeAPI.Models;

namespace ResumeAPI.Services;

public interface IResumeService
{
    Task<Guid> DuplicateResume(ResumeTreeNode resume, Guid userId);
    Task<ResumeTreeNode> GetFullResumeTree(ResumeTreeNode root);
}

public class ResumeService : IResumeService
{
    private readonly IResumeTree _tree;

    public ResumeService(IResumeTree tree)
    {
        _tree = tree;
    }

    public async Task<Guid> DuplicateResume(ResumeTreeNode resume, Guid userId)
    {
        resume.Id = Guid.NewGuid();
        resume.UserId = userId;

        await _tree.UpsertNode(resume);

        await DuplicateChildren(resume, userId);

        return resume.Id;
    }

    public async Task<ResumeTreeNode> GetFullResumeTree(ResumeTreeNode root)
    {
        root.Children = await _tree.GetChildren(root.Id);

        return await GetGrandChildren(root);
    }

    private async Task DuplicateChildren(ResumeTreeNode root, Guid userId)
    {
        if (root.Children == null || root.Children.Count == 0) return;

        foreach (var child in root.Children)
        {
            child.Id = Guid.NewGuid();
            child.UserId = userId;
            child.ParentId = root.Id;
            await _tree.UpsertNode(child);
            await DuplicateChildren(child, userId);
        }
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