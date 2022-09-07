namespace BasicGameFrameworkLibrary.Core.GeneratorHelpers;
public interface IBeginningDice<D, P, S> : ICommonMultiplayer<P, S>
    where P : class, IPlayerItem, new()
    where S : BasicSavedGameClass<P>, new()
    where D : IStandardDice, new() { }