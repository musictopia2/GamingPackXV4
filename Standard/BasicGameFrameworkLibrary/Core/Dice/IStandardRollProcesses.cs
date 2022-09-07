namespace BasicGameFrameworkLibrary.Core.Dice;
public interface IStandardRollProcesses
{
    Task RollDiceAsync();
    Task SelectUnSelectDiceAsync(int id);
}