namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public interface IPlayerTrick<S, T> : IPlayerSingleHand<T>
    where S : IFastEnumSimple
    where T : ITrickCard<S>, new()
{
    int TricksWon { get; set; } //this is common on all trick taking games.
}