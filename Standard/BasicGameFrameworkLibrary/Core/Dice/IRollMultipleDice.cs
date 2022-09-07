namespace BasicGameFrameworkLibrary.Core.Dice;
public interface IRollMultipleDice<T> : IAdvancedDIContainer
{
    Task ShowRollingAsync(BasicList<BasicList<T>> thisCol);
    Task SendMessageAsync(string category, BasicList<BasicList<T>> thisList);
    Task<BasicList<BasicList<T>>> GetDiceList(string content);
    BasicList<BasicList<T>> RollDice(int howManySections = 6);
}