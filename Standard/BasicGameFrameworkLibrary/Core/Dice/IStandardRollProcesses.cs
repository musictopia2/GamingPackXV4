namespace BasicGameFrameworkLibrary.Core.Dice;

public interface IStandardRollProcesses //this was created so view models don't need interfaces anymore.
{
    Task RollDiceAsync();
    Task SelectUnSelectDiceAsync(int id);
}