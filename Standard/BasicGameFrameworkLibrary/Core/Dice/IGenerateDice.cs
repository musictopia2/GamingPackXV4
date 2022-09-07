namespace BasicGameFrameworkLibrary.Core.Dice;
public interface IGenerateDice<Con> where Con : IConvertible
{
    BasicList<Con> GetPossibleList { get; }
}