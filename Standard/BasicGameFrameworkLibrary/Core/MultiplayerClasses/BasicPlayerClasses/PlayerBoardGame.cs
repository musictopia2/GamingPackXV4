namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public abstract class PlayerBoardGame<E> : SimplePlayer, IPlayerBoardGame<E>
    where E : IFastEnumColorSimple
{
    [ScoreColumn]
    public E? Color { get; set; }
    public abstract bool DidChooseColor { get; }
    public abstract void Clear();
}