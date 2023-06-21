using Nuke.Common;

namespace Nuke.BuildComponents;

public interface IHaveConfiguration : INukeBuild
{
    [Parameter]
    Configuration Configuration => TryGetValue(() => Configuration) ?? Configuration.Release; 
}