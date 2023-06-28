using Nuke.Common;
using Nuke.Common.CI.GitHubActions;

namespace Scrambles.Nuke.Support.Components.Have;

public interface IHaveGitHubToken : INukeBuild
{
    [Parameter] [Secret] string GitHubToken => TryGetValue(() => GitHubToken) ?? GitHubActions.Instance.Token;
}