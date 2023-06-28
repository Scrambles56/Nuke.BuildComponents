// Copyright 2023 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Nuke.BuildComponents;

[PublicAPI]
public interface IRestore : IClean
{
    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .Apply(RestoreSettingsBase)
                .Apply(RestoreSettings));
        });

    sealed Configure<DotNetRestoreSettings> RestoreSettingsBase => _ => _
        .SetProjectFile(Solution)
        .SetIgnoreFailedSources(IgnoreFailedSources);
    // RestorePackagesWithLockFile
    // .SetProperty("RestoreLockedMode", true));

    Configure<DotNetRestoreSettings> RestoreSettings => _ => _;

    [Parameter("Ignore unreachable sources during " + nameof(Restore))]
    bool IgnoreFailedSources => TryGetValue<bool?>(() => IgnoreFailedSources) ?? false;
}