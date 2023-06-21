using Nuke.Common.ProjectModel;
using Nuke.Common;

namespace Nuke.BuildComponents;

public interface IHaveSolution : INukeBuild
{
    [Solution] Solution Solution => TryGetValue(() => Solution) ?? throw new Exception("Solution is required.");
}