﻿// Copyright 2023 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.ProjectModel;

namespace Nuke.BuildComponents.Have;

[PublicAPI]
public interface IHaveSolution : INukeBuild
{
    [Solution] [Required] Solution Solution => TryGetValue(() => Solution);
}