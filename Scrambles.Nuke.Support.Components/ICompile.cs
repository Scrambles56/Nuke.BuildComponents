﻿// Copyright 2023 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

 using JetBrains.Annotations;
 using Nuke.Common;
 using Nuke.Common.ProjectModel;
 using Nuke.Common.Tooling;
 using Nuke.Common.Tools.DotNet;
 using Nuke.Common.Utilities.Collections;
 using Scrambles.Nuke.Support.Components.Have;
 using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Scrambles.Nuke.Support.Components;


[PublicAPI]
public record PublishConfiguration(Project Project, string Framework);

[PublicAPI]
public interface ICompile : IRestore, IHaveArtifacts
{
    Target Compile => _ => _
        .DependsOn(Restore)
        .WhenSkipped(DependencyBehavior.Skip)
        .Executes(() =>
        {
            ReportSummary(_ => _
                .WhenNotNull(this as IHaveGitVersion, (_, o) => _
                    .AddPair("Version", o.Versioning.NuGetVersionV2)));

            DotNetBuild(_ => _
                .Apply(CompileSettingsBase)
                .Apply(CompileSettings));

            DotNetPublish(_ => _
                    .Apply(PublishSettingsBase)
                    .Apply(PublishSettings)
                    .CombineWith(PublishConfigurations, (_, v) => _
                        .Apply(PublishProjectSettingsBase, v)
                        .Apply(PublishProjectSettings, v)),
                PublishDegreeOfParallelism);
        });

    sealed Configure<DotNetBuildSettings> CompileSettingsBase => _ => _
        .SetProjectFile(Solution)
        .SetConfiguration(Configuration)
        .When(IsServerBuild, _ => _
            .EnableContinuousIntegrationBuild())
        .SetNoRestore(SucceededTargets.Contains(Restore))
        .WhenNotNull(this as IHaveGitRepository, (_, o) => _
            .SetRepositoryUrl(o.GitRepository.HttpsUrl))
        .WhenNotNull(this as IHaveGitVersion, (_, o) => _
            .SetAssemblyVersion(o.Versioning.AssemblySemVer)
            .SetFileVersion(o.Versioning.AssemblySemFileVer)
            .SetInformationalVersion(o.Versioning.InformationalVersion));

    sealed Configure<DotNetPublishSettings> PublishSettingsBase => _ => _
        .SetConfiguration(Configuration)
        .EnableNoBuild()
        .EnableNoLogo()
        .When(IsServerBuild, _ => _
            .EnableContinuousIntegrationBuild())
        .WhenNotNull(this as IHaveGitRepository, (_, o) => _
            .SetRepositoryUrl(o.GitRepository.HttpsUrl))
        .WhenNotNull(this as IHaveGitVersion, (_, o) => _
            .SetAssemblyVersion(o.Versioning.AssemblySemVer)
            .SetFileVersion(o.Versioning.AssemblySemFileVer)
            .SetInformationalVersion(o.Versioning.InformationalVersion));
    
    sealed Configure<DotNetPublishSettings, PublishConfiguration> PublishProjectSettingsBase => (_, p) => _
        .SetProject(p.Project)
        .SetFramework(p.Framework)
        .SetOutput(ArtifactsDirectory / "apps" / p.Project.Name / p.Framework);

    Configure<DotNetBuildSettings> CompileSettings => _ => _;
    Configure<DotNetPublishSettings> PublishSettings => _ => _;
    Configure<DotNetPublishSettings, PublishConfiguration> PublishProjectSettings => (_, p) => _;

    IEnumerable<PublishConfiguration> PublishConfigurations
        => Array.Empty<PublishConfiguration>();

    int PublishDegreeOfParallelism => 1;
}