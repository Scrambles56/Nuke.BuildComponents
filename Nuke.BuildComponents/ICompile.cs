using Nuke.Common;
using Nuke.Common.Tools.DotNet;

namespace Nuke.BuildComponents;

public interface ICompile : IRestore
{
    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
            );
        });
}