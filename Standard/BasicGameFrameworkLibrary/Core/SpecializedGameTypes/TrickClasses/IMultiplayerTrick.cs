namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;

public interface IMultiplayerTrick<S, T, P>
    where S : IFastEnumSimple
    where T : ITrickCard<S>, new()
    where P : IPlayerSingleHand<T>, new()
{
    BasicList<TrickCoordinate>? ViewList { get; set; }
    P GetSpecificPlayer(int id);
}