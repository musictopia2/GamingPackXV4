namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public abstract partial class PlayerBoardGame<E> : SimplePlayer, IPlayerBoardGame<E>
    where E : IFastEnumColorSimple
{
    //may be iffy if change tracking is required here.
    [ScoreColumn]
    public E? Color { get; set; }
    public abstract bool DidChooseColor { get; }
    public abstract void Clear();
}