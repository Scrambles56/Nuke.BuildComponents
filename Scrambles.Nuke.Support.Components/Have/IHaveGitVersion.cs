// Copyright 2023 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tools.GitVersion;

namespace Scrambles.Nuke.Support.Components.Have;

[PublicAPI]
public interface IHaveGitVersion : INukeBuild
{
    [GitVersion(NoFetch = true)]
    [Required]
    GitVersion Versioning => TryGetValue(() => Versioning);
}