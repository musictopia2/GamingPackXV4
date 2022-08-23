namespace BasicGameFrameworkLibrary.Core.Dice;

public interface IDiceContainer<T> : IGenerateDice<T>, IAdvancedDIContainer where T : IConvertible { } //this is needed so something can refer to both
