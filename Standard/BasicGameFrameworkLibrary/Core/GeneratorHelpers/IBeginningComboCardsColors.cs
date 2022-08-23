namespace BasicGameFrameworkLibrary.Core.GeneratorHelpers;

public interface IBeginningComboCardsColors<E, D, P, S> : IBeginningCards<D, P, S>, IBeginningColors<E, P, S>
     where D : IDeckObject, new()
    where P : class, IPlayerBoardGame<E>, new()
    where S : BasicSavedCardClass<P, D>, new()
    where E : IFastEnumColorSimple
{
}