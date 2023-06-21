using Nuke.Common;
using Nuke.Common.Tools.DotNet;

namespace Nuke.BuildComponents;

public interface IClean : IHaveSolution, IHaveConfiguration
{
    Target Clean => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration)
            );
        });
}