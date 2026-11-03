namespace GoFish.Core.Logic;
public interface IAskProcesses
{
    void LoadAskList();
    Task NumberToAskAsync(EnumRegularCardValueList asked);
}