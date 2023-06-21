using Nuke.Common;
using Nuke.Common.Tools.DotNet;

namespace Nuke.BuildComponents;

public interface IRestore : IClean
{
    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(_ => _
                .SetProjectFile(Solution)
            );
        });
}