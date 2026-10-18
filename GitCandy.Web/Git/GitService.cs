using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using GitCandy.Base;
using GitCandy.Git.Cache;
using GitCandy.Models;
using GitCandy.Web;
using GitCandy.Web.Extensions;
using LibGit2Sharp;
using NewLife;
using NewLife.Log;

namespace GitCandy.Git;

/// <summary>Git服务。封装LibGit2Sharp操作，提供Git仓库浏览、比较、归档等核心功能</summary>
public class GitService : IDisposable
{
    public const String UnknowString = "<Unknow>";

    private readonly Repository _repository;
    private readonly String _repositoryPath;
    private readonly String _repoId = null;
    private readonly Lazy<Encoding> _i18n;
    private Boolean _disposed;

    public Encoding I18n => _i18n.Value;
    public String Owner { get; private set; }
    public String Name { get; private set; }
    public Repository Repository => _repository;

    public GitService(String owner, String name)
    {
        var info = GetPath(owner, name).AsDirectory();
        _repositoryPath = info.FullName;
        Name = info.Name;
        Owner = owner;
        _repoId = $"{Owner}\\{Name}";

        // 如果版本库无效，则创建
        if (!Repository.IsValid(_repositoryPath)) CreateRepository(owner, name);

        _repository = new Repository(_repositoryPath);

        // 延迟加载编码
        _i18n = new Lazy<Encoding>(() =>
        {
            var entry = _repository.Config.Get<String>("i18n.commitEncoding");
            return entry == null
                ? null
                : CpToEncoding(entry.Value);
        });
    }

    #region Git Smart HTTP Transport
    /// <summary>获取Git引用公告（info/refs）</summary>
    /// <param name="service">服务类型（git-upload-pack/git-receive-pack）</param>
    /// <param name="inStream">输入流</param>
    /// <param name="outStream">输出流</param>
    /// <returns></returns>
    public async Task InfoRefs(String service, Stream inStream, Stream outStream)
    {
        Contract.Requires(service == "receive-pack" || service == "upload-pack");
        await RunGitCmdAsync(service, true, inStream, outStream);
    }

    /// <summary>执行Git pack传输（git-upload-pack/git-receive-pack）</summary>
    /// <param name="service">服务类型</param>
    /// <param name="inStream">输入流</param>
    /// <param name="outStream">输出流</param>
    /// <returns></returns>
    public async Task ExecutePack(String service, Stream inStream, Stream outStream)
    {
        Contract.Requires(service == "receive-pack" || service == "upload-pack");
        await RunGitCmdAsync(service, false, inStream, outStream);
    }
    #endregion

    #region Repository Browser
    /// <summary>获取代码树。按路径浏览目录结构和文件列表</summary>
    /// <param name="path">路径（可为空或分支/标签/提交SHA）</param>
    /// <returns></returns>
    public TreeModel GetTree(String path)
    {
        var isEmptyPath = String.IsNullOrEmpty(path);
        var commit = GetCommitByPath(ref path, out var referenceName);
        if (commit == null)
        {
            if (isEmptyPath)
            {
                var branch = _repository.Branches["master"] ?? _repository.Branches.FirstOrDefault();
                return new TreeModel
                {
                    ReferenceName = branch == null ? "HEAD" : branch.FriendlyName,
                };
            }
            return null;
        }

        // 基本信息
        var model = new TreeModel
        {
            Owner = Owner,
            Name = Name,
            ReferenceName = referenceName,
            Path = String.IsNullOrEmpty(path) ? "" : path,
            Commit = new CommitModel
            {
                Sha = commit.Sha,
                Author = commit.Author,
                Committer = commit.Committer,
                CommitMessageShort = commit.MessageShort.RepetitionIfEmpty(UnknowString),
                Parents = commit.Parents.Select(s => s.Sha).ToArray()
            },
        };

        // 树结构
        var tree = String.IsNullOrEmpty(path)
            ? commit.Tree
            : commit[path] == null
                ? null
                : commit[path].Target as Tree;
        if (tree == null) return null;

        var entries = (from entry in tree
                           //join item in items on entry.Name equals item.Name into g
                           //from item in g
                       select new TreeEntryModel
                       {
                           Name = entry.Name,
                           Path = entry.Path.Replace('\\', '/'),
                           Commit = new CommitModel
                           {
                               //Sha = item.CommitSha,
                               //CommitMessageShort = item.MessageShort,
                               //Author = CreateSafeSignature(item.AuthorName, item.AuthorEmail, item.AuthorWhen),
                               //Committer = CreateSafeSignature(item.CommitterName, item.CommitterEmail, item.CommitterWhen),
                           },
                           //Sha = item.CommitSha,
                           EntryType = entry.TargetType,
                       })
                       .OrderBy(s => s.EntryType == TreeEntryTargetType.Blob)
                       .ThenBy(s => s.Name, new StringLogicalComparer())
                       .ToList();

        model.Entries = entries;

        // 缓存加载摘要
        var cfg = GitSetting.Current;
        if (cfg.AllowSummary)
        {
            var summaryAccessor = GitCacheAccessor.Singleton(new SummaryAccessor(_repoId, _repository, commit, tree));
            var items = summaryAccessor.Result.Value;
            foreach (var entry in entries)
            {
                var item = items.FirstOrDefault(e => e.Name == entry.Name);
                if (item != null)
                {
                    entry.Commit = new CommitModel
                    {
                        Sha = item.CommitSha,
                        CommitMessageShort = item.MessageShort,
                        Author = CreateSafeSignature(item.AuthorName, item.AuthorEmail, item.AuthorWhen),
                        Committer = CreateSafeSignature(item.CommitterName, item.CommitterEmail, item.CommitterWhen),
                    };
                    entry.Sha = item.CommitSha;
                }
            }
        }

        // 加载说明文件
        model.Readme = entries.FirstOrDefault(s => s.EntryType == TreeEntryTargetType.Blob
            && (s.Name.EqualIgnoreCase($"readme.{CultureInfo.CurrentCulture.Name}.md", "readme", "readme.md")));

        if (model.Readme != null)
        {
            var rm = model.Readme;
            var entry = tree[rm.Name];
            var blob = (Blob)entry.Target;
            var data = blob.GetContentStream().ToBytes();
            var encoding = FileHelper.DetectEncoding(data, CpToEncoding(commit.Encoding), _i18n.Value);
            if (encoding == null)
            {
                rm.BlobType = BlobType.Binary;
            }
            else if (rm.Name.EndsWithIgnoreCase(".md"))
            {
                rm.BlobType = BlobType.MarkDown;
                //rm.TextContent = FileHelper.ReadMarkdown(data, encoding, $"{model.ReferenceName ?? model.Commit.Sha}/{entry.Path}");
                //rm.Path = $"{model.ReferenceName ?? model.Commit.Sha}/{entry.Path}";
                rm.TextContent = data.ToStr(encoding);
                rm.TextBrush = "no-highlight";
            }
            else
            {
                rm.BlobType = BlobType.Text;
                rm.TextContent = data.ToStr(encoding);
                rm.TextBrush = "no-highlight";
            }
        }

        model.BranchSelector = GetBranchSelectorModel(referenceName, commit.Sha, path);
        model.PathBar = new PathBarModel
        {
            Name = Name,
            Action = "Tree",
            Path = path,
            ReferenceName = referenceName,
            ReferenceSha = commit.Sha,
            HideLastSlash = false,
        };

        if (model.IsRoot)
        {
            var scopeAccessor = GitCacheAccessor.Singleton(new ScopeAccessor(_repoId, _repository, commit));
            model.Scope = scopeAccessor.Result.Value;
        }

        return model;
    }

    /// <summary>获取文件内容（Blob视图）。返回文件元数据、内容和编码</summary>
    /// <param name="path">文件路径（含分支/标签/提交SHA）</param>
    /// <returns></returns>
    public TreeEntryModel GetBlob(String path)
    {
        var commit = GetCommitByPath(ref path, out var referenceName);
        if (commit == null) return null;

        var entry = commit[path];
        if (entry == null || entry.TargetType != TreeEntryTargetType.Blob) return null;

        var blob = (Blob)entry.Target;

        var cacheAccessor = GitCacheAccessor.Singleton(new LastCommitAccessor(_repoId, _repository, commit, path));
        var lastCommitSha = cacheAccessor.Result.Value;
        if (!lastCommitSha.IsNullOrEmpty() && lastCommitSha != commit.Sha)
            commit = _repository.Lookup<Commit>(lastCommitSha) ?? commit;

        var data = blob.GetContentStream().ToBytes();
        var encoding = FileHelper.DetectEncoding(data, CpToEncoding(commit.Encoding), _i18n.Value);
        var extension = Path.GetExtension(entry.Name).ToLower();
        var model = new TreeEntryModel
        {
            Name = entry.Name,
            ReferenceName = referenceName,
            Sha = commit.Sha,
            Path = String.IsNullOrEmpty(path) ? "" : path,
            Commit = new CommitModel
            {
                Sha = commit.Sha,
                Author = commit.Author,
                Committer = commit.Committer,
                CommitMessage = commit.Message.RepetitionIfEmpty(UnknowString),
                CommitMessageShort = commit.MessageShort.RepetitionIfEmpty(UnknowString),
                Parents = commit.Parents.Select(s => s.Sha).ToArray()
            },
            EntryType = entry.TargetType,
            RawData = data,
            SizeString = FileHelper.GetSizeString(data.Length),
            TextContent = data.ToStr(encoding),
            TextBrush = FileHelper.BrushMapping.ContainsKey(extension)
                ? FileHelper.BrushMapping[extension]
                : "no-highlight",
            BlobType = encoding == null
                ? FileHelper.ImageSet.Contains(extension)
                    ? BlobType.Image
                    : BlobType.Binary
                : extension == ".md"
                    ? BlobType.MarkDown
                    : BlobType.Text,
            BlobEncoding = encoding,
            BranchSelector = GetBranchSelectorModel(referenceName, commit.Sha, path),
            PathBar = new PathBarModel
            {
                Name = Name,
                Action = "Tree",
                Path = path,
                ReferenceName = referenceName,
                ReferenceSha = commit.Sha,
                HideLastSlash = true,
            },
        };

        return model;
    }

    /// <summary>获取单次提交详情</summary>
    /// <param name="path">提交SHA</param>
    /// <returns></returns>
    public CommitModel GetCommit(String path)
    {
        var commit = GetCommitByPath(ref path, out var referenceName);
        if (commit == null) return null;

        var treeEntry = commit[path];
        var isBlob = treeEntry != null && treeEntry.TargetType == TreeEntryTargetType.Blob;
        var model = ToCommitModel(commit, referenceName, !isBlob, path);
        model.PathBar = new PathBarModel
        {
            Name = Name,
            Action = "Commit",
            Path = path,
            ReferenceName = referenceName,
            ReferenceSha = commit.Sha,
            HideLastSlash = isBlob,
        };
        return model;
    }

    /// <summary>比较两个分支/提交之间的差异</summary>
    /// <param name="start">起始分支/提交</param>
    /// <param name="end">目标分支/提交</param>
    /// <returns></returns>
    public CompareModel GetCompare(String start, String end)
    {
        var commit1 = GetCommitByPath(ref start, out var name1);
        var commit2 = GetCommitByPath(ref end, out var name2);
        if (commit1 == null)
        {
            commit1 = _repository.Head.Tip;
            name1 = _repository.Head.FriendlyName;
        }
        if (commit2 == null)
        {
            commit2 = _repository.Head.Tip;
            name2 = _repository.Head.FriendlyName;
        }

        var walks = _repository.Commits
            .QueryBy(new CommitFilter
            {
                IncludeReachableFrom = commit2,
                ExcludeReachableFrom = commit1,
                SortBy = CommitSortStrategies.Time
            })
            .Select(s => new CommitModel
            {
                Sha = s.Sha,
                Committer = s.Committer,
                CommitMessageShort = s.MessageShort.RepetitionIfEmpty(UnknowString),
            })
            .ToArray();

        var fromBranchSelector = GetBranchSelectorModel(name1, commit1.Sha, null);
        var toBranchSelector = GetBranchSelectorModel(name2, commit2.Sha, null);
        var model = new CompareModel
        {
            BaseBranchSelector = fromBranchSelector,
            CompareBranchSelector = toBranchSelector,
            CompareResult = ToCommitModel(commit1, name1, true, "", commit2.Tree),
            Walks = walks,
        };
        return model;
    }

    /// <summary>获取提交历史列表（分页）</summary>
    /// <param name="path">分支/标签/路径</param>
    /// <param name="page">页码</param>
    /// <param name="pagesize">每页大小</param>
    /// <returns></returns>
    public CommitsModel GetCommits(String path, Int32 page = 1, Int32 pagesize = 20)
    {
        var cfg = GitSetting.Current;
        if (!cfg.AllowCommits) return null;

        var commit = GetCommitByPath(ref path, out var referenceName);
        if (commit == null) return null;

        var commitsAccessor = GitCacheAccessor.GetOrAdd(new CommitsAccessor(_repoId, _repository, commit, path, page, pagesize));
        var scopeAccessor = GitCacheAccessor.Singleton(new ScopeAccessor(_repoId, _repository, commit, path));

        var model = new CommitsModel
        {
            ReferenceName = referenceName,
            Sha = commit.Sha,
            Commits = commitsAccessor.Result.Value
                .Select(s => new CommitModel
                {
                    CommitMessageShort = s.MessageShort,
                    Sha = s.CommitSha,
                    Author = CreateSafeSignature(s.AuthorName, s.AuthorEmail, s.AuthorWhen),
                    Committer = CreateSafeSignature(s.CommitterName, s.CommitterEmail, s.CommitterWhen),
                })
                .ToList(),
            CurrentPage = page,
            ItemCount = scopeAccessor.Result.Value.Commits,
            Path = String.IsNullOrEmpty(path) ? "" : path,
            PathBar = new PathBarModel
            {
                Name = Name,
                Action = "Commits",
                Path = path,
                ReferenceName = referenceName,
                ReferenceSha = commit.Sha,
                HideLastSlash = true, // I want a improvement here
            },
        };

        return model;
    }

    /// <summary>获取Blame数据。逐行查看文件的修改历史和责任人</summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    public BlameModel GetBlame(String path)
    {
        var cfg = GitSetting.Current;
        if (!cfg.AllowBlame) return null;

        var commit = GetCommitByPath(ref path, out var referenceName);
        if (commit == null) return null;

        var entry = commit[path];
        if (entry == null || entry.TargetType != TreeEntryTargetType.Blob) return null;

        var blob = (Blob)entry.Target;

        var accessor = GitCacheAccessor.Singleton(new BlameAccessor(_repoId, _repository, commit, path, CpToEncoding(commit.Encoding), _i18n.Value));
        var hunks = accessor.Result.Value;

        var model = new BlameModel
        {
            ReferenceName = referenceName,
            Sha = commit.Sha,
            Path = String.IsNullOrEmpty(path) ? "" : path,
            SizeString = FileHelper.GetSizeString(blob.Size),
            Brush = FileHelper.GetBrush(path),
            Hunks = hunks,
            BranchSelector = GetBranchSelectorModel(referenceName, commit.Sha, path),
            PathBar = new PathBarModel
            {
                Name = Name,
                Action = "Tree",
                Path = path,
                ReferenceName = referenceName,
                ReferenceSha = commit.Sha,
                HideLastSlash = true,
            },
        };

        return model;
    }

    /// <summary>获取归档文件名。按分支/标签/提交打包</summary>
    /// <param name="path">分支/标签/提交</param>
    /// <param name="referenceName">引用名称</param>
    /// <returns></returns>
    public String GetArchiveFilename(String path, out String referenceName)
    {
        referenceName = null;

        var cfg = GitSetting.Current;
        if (!cfg.AllowArchive) return null;

        var commit = GetCommitByPath(ref path, out referenceName);
        if (commit == null) return null;

        referenceName ??= commit.Sha;

        var accessor = GitCacheAccessor.Singleton(new ArchiverAccessor(_repoId, _repository, commit, CpToEncoding(commit.Encoding), _i18n.Value));

        return accessor.Result.Value;
    }

    /// <summary>获取标签列表</summary>
    /// <returns></returns>
    public TagsModel GetTags()
    {
        var model = new TagsModel
        {
            Tags = (from tag in _repository.Tags
                    let commit = (tag.IsAnnotated ? tag.Annotation.Target : tag.Target) as Commit
                    where commit != null
                    select new TagModel
                    {
                        ReferenceName = tag.FriendlyName,
                        Sha = tag.Target.Sha,
                        When = ((Commit)tag.Target).Author.When,
                        MessageShort = ((Commit)tag.Target).MessageShort.RepetitionIfEmpty(UnknowString),
                    })
                    .OrderByDescending(s => s.When)
                    .ToArray()
        };
        return model;
    }

    /// <summary>获取分支列表</summary>
    /// <returns></returns>
    public BranchesModel GetBranches()
    {
        var cfg = GitSetting.Current;
        if (!cfg.AllowHistoryDivergence) return new BranchesModel();

        var head = _repository.Head;
        if (head.Tip == null) return new BranchesModel();

        var key = CalcBranchesKey();
        var accessor = GitCacheAccessor.Singleton(new HistoryDivergenceAccessor(_repoId, _repository, key));
        var aheadBehinds = accessor.Result.Value;
        var model = new BranchesModel
        {
            Commit = ToCommitModel(head.Tip, head.FriendlyName),
            AheadBehinds = aheadBehinds.Select(s => new AheadBehindModel
            {
                Ahead = s.Ahead,
                Behind = s.Behind,
                Commit = new CommitModel
                {
                    ReferenceName = s.Name,
                    Author = CreateSafeSignature(s.AuthorName, s.AuthorEmail, s.AuthorWhen),
                    Committer = CreateSafeSignature(s.CommitterName, s.CommitterEmail, s.CommitterWhen),
                },
            }).ToArray(),
        };
        return model;
    }

    /// <summary>删除分支</summary>
    /// <param name="branch">分支名</param>
    public void DeleteBranch(String branch) => _repository.Branches.Remove(branch);

    /// <summary>删除标签</summary>
    /// <param name="tag">标签名</param>
    public void DeleteTag(String tag) => _repository.Tags.Remove(tag);

    /// <summary>获取贡献者统计</summary>
    /// <param name="path">分支/标签</param>
    /// <returns></returns>
    public ContributorsModel GetContributors(String path)
    {
        var cfg = GitSetting.Current;
        if (!cfg.AllowContributors) return null;

        var commit = GetCommitByPath(ref path, out var referenceName);
        if (commit == null) return null;

        var contributorsAccessor = GitCacheAccessor.Singleton(new ContributorsAccessor(_repoId, _repository, commit));
        var contributors = contributorsAccessor.Result.Value;
        contributors.OrderedCommits = contributors.OrderedCommits
            .Take(GitSetting.Current.Contributors)
            .ToArray();
        var statistics = new RepositoryStatisticsModel
        {
            Current = contributors
        };
        statistics.Current.Branch = referenceName;

        if (_repository.Head.Tip != commit)
        {
            contributorsAccessor = GitCacheAccessor.Singleton(new ContributorsAccessor(_repoId, _repository, _repository.Head.Tip));
            statistics.Default = contributorsAccessor.Result.Value;
            statistics.Default.Branch = _repository.Head.FriendlyName;
        }

        var key = CalcBranchesKey(true);
        var repositorySizeAccessor = GitCacheAccessor.Singleton(new RepositorySizeAccessor(_repoId, _repository, key));
        statistics.RepositorySize = repositorySizeAccessor.Result.Value;

        var model = new ContributorsModel
        {
            Name = Name,
            Statistics = statistics,
        };
        return model;
    }
    #endregion

    #region Repository Settings
    /// <summary>获取当前HEAD分支名</summary>
    /// <returns></returns>
    public String GetHeadBranch()
    {
        var head = _repository.Head;
        if (head == null)
            return null;
        return head.FriendlyName;
    }

    /// <summary>获取所有本地分支名列表</summary>
    /// <returns></returns>
    public String[] GetLocalBranches() => _repository.Branches.Select(s => s.FriendlyName).OrderBy(s => s, new StringLogicalComparer()).ToArray();

    /// <summary>设置默认分支（HEAD）</summary>
    /// <param name="name">分支名</param>
    /// <returns></returns>
    public Boolean SetHeadBranch(String name)
    {
        var refs = _repository.Refs;
        var refer = refs["refs/heads/" + (name ?? "master")];
        if (refer == null)
            return false;
        refs.UpdateTarget(refs.Head, refer);
        return true;
    }
    #endregion

    #region Private Methods
    private BranchSelectorModel GetBranchSelectorModel(String referenceName, String refer, String path)
    {
        var model = new BranchSelectorModel
        {
            Branches = _repository.Branches.Select(s => s.FriendlyName).OrderBy(s => s, new StringLogicalComparer()).ToList(),
            Tags = _repository.Tags.Select(s => s.FriendlyName).OrderByDescending(s => s, new StringLogicalComparer()).ToList(),
            Current = referenceName ?? refer.ToShortSha(),
            Path = path,
        };
        model.CurrentIsBranch = model.Branches.Contains(referenceName) || !model.Tags.Contains(referenceName);

        return model;
    }

    private Commit GetCommitByPath(ref String path, out String referenceName)
    {
        referenceName = null;

        if (String.IsNullOrEmpty(path))
        {
            referenceName = _repository.Head.FriendlyName;
            path = "";
            return _repository.Head.Tip;
        }

        path = path + "/";
        var p = path;
        var branch = _repository.Branches.FirstOrDefault(s => p.StartsWith(s.FriendlyName + "/"));
        if (branch != null && branch.Tip != null)
        {
            referenceName = branch.FriendlyName;
            path = path[referenceName.Length..].Trim('/');
            return branch.Tip;
        }

        var tag = _repository.Tags.FirstOrDefault(s => p.StartsWith(s.FriendlyName + "/"));
        if (tag != null && tag.Target is Commit)
        {
            referenceName = tag.FriendlyName;
            path = path[referenceName.Length..].Trim('/');
            return (Commit)tag.Target;
        }

        var index = path.IndexOf('/');
        var commit = _repository.Lookup<Commit>(path[..index]);
        path = path[(index + 1)..].Trim('/');
        return commit;
    }

    private CommitModel ToCommitModel(Commit commit, String referenceName, Boolean isTree = true, String detailFilter = null, Tree compareWith = null)
    {
        if (commit == null)
            return null;

        var model = new CommitModel
        {
            ReferenceName = referenceName,
            Sha = commit.Sha,
            CommitMessageShort = commit.MessageShort.RepetitionIfEmpty(UnknowString),
            CommitMessage = commit.Message.RepetitionIfEmpty(UnknowString),
            Author = commit.Author,
            Committer = commit.Committer,
            Parents = commit.Parents.Select(e => e.Sha).ToArray(),
        };
        if (detailFilter != null)
        {
            if (detailFilter != "" && isTree)
                detailFilter = detailFilter + "/";
            var firstTree = compareWith != null
                ? commit.Tree
                : commit.Parents.Any()
                    ? commit.Parents.First().Tree
                    : null;
            if (compareWith == null)
                compareWith = commit.Tree;
            var compareOptions = new LibGit2Sharp.CompareOptions
            {
                Similarity = SimilarityOptions.Renames,
            };
            var paths = detailFilter == ""
                ? null
                : new[] { detailFilter };
            var changes = _repository.Diff.Compare<TreeChanges>(firstTree, compareWith, paths, compareOptions: compareOptions);
            var patches = _repository.Diff.Compare<Patch>(firstTree, compareWith, paths, compareOptions: compareOptions);
            model.Changes = (from s in changes
                             where (s.Path.Replace('\\', '/') + '/').StartsWith(detailFilter)
                             orderby s.Path
                             let patch = patches[s.Path]
                             select new CommitChangeModel
                             {
                                 //Name = s.Name,
                                 OldPath = s.OldPath.Replace('\\', '/'),
                                 Path = s.Path.Replace('\\', '/'),
                                 ChangeKind = s.Status,
                                 LinesAdded = patch.LinesAdded,
                                 LinesDeleted = patch.LinesDeleted,
                                 Patch = patch.Patch,
                             })
                             .ToArray();
        }

        return model;
    }

    private Encoding CpToEncoding(String encoding)
    {
        try
        {
            if (encoding.StartsWith("cp", StringComparison.OrdinalIgnoreCase))
                return Encoding.GetEncoding(Int32.Parse(encoding[2..]));

            return Encoding.GetEncoding(encoding);
        }
        catch
        {
            return null;
        }
    }

    private String CalcBranchesKey(Boolean includeTags = false)
    {
        var sb = new StringBuilder();
        var head = _repository.Head;
        sb.Append(":");
        sb.Append(head.CanonicalName);
        if (head.Tip != null)
            sb.Append(head.Tip.Sha);
        sb.Append(';');
        foreach (var branch in _repository.Branches.OrderBy(s => s.CanonicalName))
        {
            sb.Append(':');
            sb.Append(branch.CanonicalName);
            if (branch.Tip != null)
                sb.Append(branch.Tip.Sha);
        }
        if (includeTags)
        {
            sb.Append(';');
            foreach (var tag in _repository.Tags.OrderBy(s => s.CanonicalName))
            {
                sb.Append(':');
                sb.Append(tag.CanonicalName);
                if (tag.Target != null)
                    sb.Append(tag.Target.Sha);
            }
        }
        return sb.ToString();
    }

    private Signature CreateSafeSignature(String name, String email, DateTimeOffset when) => new(name.RepetitionIfEmpty(UnknowString), email.RepetitionIfEmpty(UnknowString), when);
    #endregion

    #region Static Methods
    /// <summary>获取版本库的路径</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <returns></returns>
    public static String GetPath(String owner, String name)
    {
        var p = GitSetting.Current.RepositoryPath;
        p = p.CombinePath(owner, name).GetFullPath();

        return p;
    }

    /// <summary>创建Git仓库。可选从远程导入</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <param name="remoteUrl">远程导入地址（可选）</param>
    /// <returns></returns>
    public static Boolean CreateRepository(String owner, String name, String remoteUrl = null)
    {
        var path = GetPath(owner, name);
        try
        {
            using (var repo = new Repository(Repository.Init(path, true)))
            {
                repo.Config.Set("core.logallrefupdates", true);
                if (remoteUrl != null)
                {
                    repo.Network.Remotes.Add("origin", remoteUrl, "+refs/*:refs/*");
                    Task.Run(() =>
                    {
                        XTrace.WriteLine("[{0}/{1}]准备从远程拉取 {2}", owner, name, remoteUrl);
                        try
                        {
                            var sw = Stopwatch.StartNew();
                            using (var fetch_repo = new Repository(repo.Info.Path))
                            {
                                //fetch_repo.Fetch("origin");
                                fetch_repo.Network.Fetch("origin", Array.Empty<String>());
                            }
                            sw.Stop();
                            XTrace.WriteLine("远程拉取成功，耗时 {0:n0}毫秒", sw.ElapsedMilliseconds);
                        }
                        catch (Exception ex)
                        {
                            XTrace.WriteException(ex);
                        }
                    });
                }
            }
            return true;
        }
        catch
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch { }
            return false;
        }
    }

    /// <summary>删除Git仓库（包含裸仓库目录）</summary>
    /// <param name="owner">拥有者</param>
    /// <param name="name">仓库名</param>
    /// <returns></returns>
    public static Boolean DeleteRepository(String owner, String name)
    {
        var path = GetPath(owner, name);
        var temp = path + "." + DateTime.Now.Ticks + ".del";

        var retry = 3;
        for (; retry > 0; retry--)
            try
            {
                Directory.Move(path, temp);
                break;
            }
            catch
            {
                Task.Delay(1000).Wait();
            }

        for (; retry > 0; retry--)
            try
            {
                var di = new DirectoryInfo(temp);

                foreach (var info in di.GetFileSystemInfos("*", SearchOption.AllDirectories))
                    info.Attributes = FileAttributes.Archive;

                break;
            }
            catch
            {
                Task.Delay(1000).Wait();
            }

        for (; retry > 0; retry--)
            try
            {
                Directory.Delete(temp, true);
                break;
            }
            catch
            {
                Task.Delay(1000).Wait();
            }

        return retry > 0;
    }

    //private static DirectoryInfo GetDirectoryInfo(String project)
    //{
    //    return new DirectoryInfo(Path.Combine(UserConfiguration.Current.RepositoryPath.GetFullPath(), project));
    //}
    #endregion

    #region RunGitCmd
    // un-safe implementation
    private void RunGitCmd(String serviceName, Boolean advertiseRefs, Stream inStream, Stream outStream)
    {
        var args = serviceName + " --stateless-rpc";
        if (advertiseRefs)
            args += " --advertise-refs";
        args += " \"" + _repositoryPath + "\"";

        WriteLog("git.exe {0}", args);
        var sw = new Stopwatch();
        sw.Start();

        var cfg = GitSetting.Current;
        var info = new ProcessStartInfo(cfg.GetGitFile(), args)
        {
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(cfg.RepositoryPath.GetFullPath()),
        };

        var rs = 0;
        using (var process = Process.Start(info))
        {
            inStream.CopyTo(process.StandardInput.BaseStream);
            process.StandardInput.Close();
            process.StandardOutput.BaseStream.CopyTo(outStream);

            if (!process.WaitForExit(15 * 1000))
            {
                process.Kill();
            }

            rs = process.ExitCode;
        }

        sw.Stop();
        WriteLog("git.exe 完成 {0} 耗时 {1}", rs, sw.Elapsed);
    }

    private async Task RunGitCmdAsync(String serviceName, Boolean advertiseRefs, Stream inStream, Stream outStream)
    {
        var args = serviceName + " --stateless-rpc";
        if (advertiseRefs)
            args += " --advertise-refs";
        args += " \"" + _repositoryPath + "\"";

        WriteLog("git.exe {0}", args);
        var sw = new Stopwatch();
        sw.Start();

        var cfg = GitSetting.Current;
        var info = new ProcessStartInfo(cfg.GetGitFile(), args)
        {
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(cfg.RepositoryPath.GetFullPath()),
        };

        var rs = 0;
        using (var process = Process.Start(info))
        {
            await inStream.CopyToAsync(process.StandardInput.BaseStream);
            process.StandardInput.Close();
            await process.StandardOutput.BaseStream.CopyToAsync(outStream);

            if (!process.WaitForExit(15 * 1000))
            {
                process.Kill();
            }

            rs = process.ExitCode;
        }

        sw.Stop();
        WriteLog("git.exe 完成 {0} 耗时 {1}", rs, sw.Elapsed);
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(Boolean disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_repository != null)
                {
                    _repository.Dispose();
                }
            }

            _disposed = true;
        }
    }

    ~GitService()
    {
        Dispose(false);
    }
    #endregion

    #region 日志
    /// <summary>日志</summary>
    public ILog Log { get; set; } = Logger.Null;

    /// <summary>写日志</summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public void WriteLog(String format, params Object[] args)
    {
        if (Log != null && Log.Enable) Log.Info(format, args);
    }
    #endregion
}