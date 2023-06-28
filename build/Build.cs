using Scrambles.Nuke.Support.Components;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;


[GitHubActions(
    "ci", 
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new []{ nameof(IPack.Pack) },
    OnPullRequestBranches = new []{ MainBranch },
    FetchDepth = 0
)]
[GitHubActions(
    "cd",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new []{ nameof(IPublish.Publish) },
    EnableGitHubToken = true,
    OnPushBranches = new []{ MainBranch },
    OnPushTags = new []{ "prerelease-*" },
    ImportSecrets = new []
    {
        nameof(IPublish.NuGetSource),
        nameof(IPublish.NuGetApiKey)
    },
    FetchDepth = 0
)]
class Build : NukeBuild, IPublish
{
    const string MainBranch = "main";
    
    public static int Main () => Execute<Build>(x => (x as ICompile).Compile);
}
