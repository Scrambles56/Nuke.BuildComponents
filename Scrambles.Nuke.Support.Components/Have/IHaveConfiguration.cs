// Copyright 2023 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using JetBrains.Annotations;
using Nuke.Common;
using Scrambles.Nuke.Support.Components.Support;

namespace Scrambles.Nuke.Support.Components.Have;

[PublicAPI]
public interface IHaveConfiguration : INukeBuild
{
    [Parameter] Configuration Configuration => TryGetValue(() => Configuration) ??
                                               (IsLocalBuild ? Configuration.Debug : Configuration.Release);
}