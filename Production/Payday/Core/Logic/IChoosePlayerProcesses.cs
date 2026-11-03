namespace Payday.Core.Logic;
public interface IChoosePlayerProcesses
{
    Task ProcessChosenPlayerAsync();
    void LoadPlayerList();
}