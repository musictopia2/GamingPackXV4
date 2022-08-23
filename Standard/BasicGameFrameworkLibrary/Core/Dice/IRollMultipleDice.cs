namespace BasicGameFrameworkLibrary.Core.Dice;

public interface IRollMultipleDice<T> : IAdvancedDIContainer  //i think it needs to take in a type.
{
    Task ShowRollingAsync(BasicList<BasicList<T>> thisCol);
    Task SendMessageAsync(string category, BasicList<BasicList<T>> thisList); //decided to not send anything now.
    Task<BasicList<BasicList<T>>> GetDiceList(string content); //could send in a delegate but for now, don't worry about it.
    BasicList<BasicList<T>> RollDice(int howManySections = 6); //you do have to decide how many times it will do this.
}