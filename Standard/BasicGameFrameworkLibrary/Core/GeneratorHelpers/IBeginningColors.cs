namespace BasicGameFrameworkLibrary.Core.GeneratorHelpers;

public interface IBeginningColors<E, P, S> : ICommonMultiplayer<P, S>
    where E : IFastEnumColorSimple
    where P : class, IPlayerBoardGame<E>, new()
    where S : BasicSavedGameClass<P>, new()
{
}