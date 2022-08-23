namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;

public class PlayerTrick<S, T> : PlayerSingleHand<T>, IPlayerTrick<S, T>
    where S : IFastEnumSimple
    where T : ITrickCard<S>, new()
{
    [ScoreColumn]
    public int TricksWon { get; set; }
}