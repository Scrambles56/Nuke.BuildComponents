// Copyright 2023 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using JetBrains.Annotations;
using Nuke.Common;
using static Nuke.Common.ChangeLog.ChangelogTasks;

namespace Nuke.BuildComponents.Have;

[PublicAPI]
public interface IHaveChangelog : IHaveGitRepository
{
    string ChangelogFile
    {
        get
        {
            var filePath = RootDirectory / "CHANGELOG.md";
            Assert.True(File.Exists(filePath), "Change log file does not exist.");
            return filePath;
        }
    }

    string NuGetReleaseNotes => GetNuGetReleaseNotes(ChangelogFile, GitRepository);
}