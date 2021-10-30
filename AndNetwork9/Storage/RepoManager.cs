using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Storage;
using LibGit2Sharp;

namespace AndNetwork9.Storage;

public class RepoManager
{
    public const string RepoPath = "/var/lib/AndNetwork9/data";
    public static readonly Identity ServiceIdentity = new("Секретарь", "0");
    private readonly object _fileLock = new();

    public RepoManager()
    {
        Directory.CreateDirectory(RepoPath);
    }


    public void CreateRepo(in Repo repo)
    {
        string path = Path.Combine(RepoPath, repo.RepoName);
        Directory.CreateDirectory(path);
        string filePath = Path.Combine(path, ".gitignore");
        File.WriteAllLinesAsync(filePath,
            new List<string>
            {
                ".png",
                ".sbcB5",
                ".sbcB4",
            });
        Repository.Init(path);
        using IRepository repository = new Repository(path);
        Signature signature =
            new(repo.Creator is null ? ServiceIdentity : repo.Creator.GetIdentity(), DateTimeOffset.UtcNow);

        Commands.Stage(repository, filePath);
        repository.Commit("Add .gitignore", signature, signature);
    }

    public async Task LoadFile(byte[] fileBytes, RepoNode node)
    {
        string repoPath = Path.Combine(RepoPath, node.Repo.RepoName);
        string filePath = node.Repo.Type switch
        {
            RepoType.Blueprint => "bp.sbc",
            RepoType.Script => "Script.cs",
            _ => throw new ArgumentOutOfRangeException(),
        };
        filePath = Path.Combine(repoPath, filePath);
        using IRepository repository = new Repository(repoPath);
        await WriteAndCommit(repository, filePath, fileBytes, node.Description, node).ConfigureAwait(false);
    }

    public byte[] GetFile(RepoNode node)
    {
        string repoPath = Path.Combine(RepoPath, node.Repo.RepoName);
        string filePath = node.Repo.Type switch
        {
            RepoType.Blueprint => "bp.sbc",
            RepoType.Script => "Script.cs",
            _ => throw new ArgumentOutOfRangeException(),
        };
        filePath = Path.Combine(repoPath, filePath);
        using IRepository repository = new Repository(repoPath);
        Commit commit = (Commit)repository.Tags.First(x => x.FriendlyName == node.Tag).Target;
        Branch head = repository.Head;
        byte[] value;
        lock (_fileLock)
        {
            Commands.Checkout(repository, commit);
            value = File.ReadAllBytes(filePath);
            Commands.Checkout(repository, head);
        }

        return value;
    }

    private static async Task WriteAndCommit(IRepository repository, string filePath, byte[] content,
        string comment,
        RepoNode node)
    {
        if (repository is null) throw new ArgumentNullException(nameof(repository));
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
        if (content is null) throw new ArgumentNullException(nameof(content));
        if (string.IsNullOrWhiteSpace(comment)) throw new ArgumentNullException(nameof(comment));
        if (node is null) throw new ArgumentNullException(nameof(node));
        Signature signature =
            new(node.Author is null ? ServiceIdentity : node.Author.GetIdentity(), DateTimeOffset.UtcNow);
        await File.WriteAllBytesAsync(filePath, content, CancellationToken.None).ConfigureAwait(false);
        Commands.Stage(repository, filePath);
        Commit commit = repository.Commit(comment,
            signature,
            signature,
            new()
            {
                AllowEmptyCommit = true,
            });
        repository.Tags.Add(node.Tag, commit);
    }
}