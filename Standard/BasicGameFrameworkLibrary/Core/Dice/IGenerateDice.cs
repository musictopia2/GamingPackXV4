namespace BasicGameFrameworkLibrary.Core.Dice;

public interface IGenerateDice<Con> where Con : IConvertible
{
    BasicList<Con> GetPossibleList { get; } //i like the idea of it being a property (read only)
}