namespace BasicGameFrameworkLibrary.Core.Dice;

public interface IRollSingleDice<T> : IAdvancedDIContainer
{
    Task ShowRollingAsync(BasicList<T> thisCol);
    Task SendMessageAsync(string category, BasicList<T> thisList);
    Task<BasicList<T>> GetDiceList(string content);
    BasicList<T> RollDice(int howManySections = 6);
}