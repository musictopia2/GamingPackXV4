namespace LifeBoardGame.Core.Logic;
public interface ITradeSalaryProcesses
{
    Task TradedSalaryAsync(string player);
    Task ComputerTradeAsync();
    void LoadOtherPlayerSalaries();
}