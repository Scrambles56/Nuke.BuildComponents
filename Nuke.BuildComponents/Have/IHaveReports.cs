// Copyright 2023 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using JetBrains.Annotations;
using Nuke.Common.IO;

namespace Nuke.BuildComponents.Have;

[PublicAPI]
public interface IHaveReports : IHaveArtifacts
{
    AbsolutePath ReportDirectory => ArtifactsDirectory / "reports";
}