namespace BasicGameFrameworkLibrary.Core.GeneratorHelpers;
public interface IBeginningColors<E, P, S> : ICommonMultiplayer<P, S>
    where E : IFastEnumColorSimple
    where P : class, IPlayerWithColor<E>, new()
    where S : BasicSavedGameClass<P>, new() { }