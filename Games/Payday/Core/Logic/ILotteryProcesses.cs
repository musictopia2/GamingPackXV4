namespace Payday.Core.Logic;
public interface ILotteryProcesses
{
    void LoadLotteryList();
    bool CanStartLotteryProcess();
    Task ProcessLotteryAsync();
    Task RollLotteryAsync();
}