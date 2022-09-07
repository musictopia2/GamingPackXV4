namespace BasicGameFrameworkLibrary.Core.GeneratorHelpers;
public interface IBeginningCards<D, P, S> : ICommonMultiplayer<P, S>
    where D : IDeckObject, new()
    where P : class, IPlayerItem, new()
    where S : BasicSavedCardClass<P, D>, new() { }