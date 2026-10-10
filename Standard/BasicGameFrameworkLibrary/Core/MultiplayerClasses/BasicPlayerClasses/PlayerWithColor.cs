namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public abstract class PlayerWithColor<E> : SimplePlayer, IPlayerWithColor<E>
    where E : IFastEnumColorSimple
{
    [ScoreColumn]
    public E? Color { get; set; }
    public abstract bool DidChooseColor { get; }
    public abstract void Clear();
}