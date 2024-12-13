namespace BasicGameFrameworkLibrary.Core.Dice;
public interface IGenerateDice<Con> where Con : IConvertible
{
    Con GetRandomDiceValue(bool isLastItem);  // Influenced by isLastItem, but not necessarily always decisive
}