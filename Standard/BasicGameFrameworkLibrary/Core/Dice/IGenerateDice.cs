namespace BasicGameFrameworkLibrary.Core.Dice;
public interface IGenerateDice<Con> where Con : IConvertible
{
    //this works for games where you have several dice  you want to set up the weighted average system ahead of time.
    void StartRoll(); //this way some who implement the interface can decide to set up the distributions for this roll.
    Con GetRandomDiceValue(bool isLastItem);  // Influenced by isLastItem, but not necessarily always decisive
}