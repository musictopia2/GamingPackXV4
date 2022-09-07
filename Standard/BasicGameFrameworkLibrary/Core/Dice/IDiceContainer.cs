namespace BasicGameFrameworkLibrary.Core.Dice;
public interface IDiceContainer<T> : IGenerateDice<T>, IAdvancedDIContainer where T : IConvertible { }