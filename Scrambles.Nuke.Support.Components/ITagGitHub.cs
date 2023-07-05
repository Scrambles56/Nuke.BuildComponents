using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using static Nuke.Common.Tools.GitHub.GitHubTasks;
using Scrambles.Nuke.Support.Components.Have;
using Serilog;
using Committer = Octokit.Committer;
using NewReference = Octokit.NewReference;
using NewTag = Octokit.NewTag;

namespace Scrambles.Nuke.Support.Components;

public interface ITagGitHub : IPack, IHaveGitVersion, IHaveGitHubToken, IHaveGitRepository
{
    Target PushTag => _ => _
        .DependsOn<IPack>(x => x.Pack)
        .Requires(() => GitHubToken)
        .Requires(() => GitRepository)
        .Requires(() => Versioning)
        .OnlyWhenDynamic(() => GitRepository.IsOnMainOrMasterBranch())
        .Executes(async () =>
        {
            GitHubClient.Credentials = new Octokit.Credentials(GitHubToken);
            
            var repo = (await GitHubClient.Repository.Get(
                GitRepository.GetGitHubOwner(),
                GitRepository.GetGitHubName()
            )).NotNull("GitHub repository not found.")!;

            var reference = (await GitHubClient.Git.Reference.Create(
                repo.Id, 
                new NewReference($"refs/tags/{Versioning.FullSemVer}", Versioning.Sha)
            )).NotNull("Reference should exist");
            
            Log.Information("Created tag {Tag} on {Repo}", reference.Ref, repo.FullName);
        });
}