// Copyright 2023 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;

namespace Nuke.BuildComponents.Have;

[PublicAPI]
public interface IHaveGitRepository : INukeBuild
{
    [GitRepository] [Required] GitRepository GitRepository => TryGetValue(() => GitRepository);
}