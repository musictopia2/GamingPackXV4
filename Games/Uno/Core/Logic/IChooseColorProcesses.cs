namespace Uno.Core.Logic;
public interface IChooseColorProcesses
{
    Task ColorChosenAsync(EnumColorTypes color);
}