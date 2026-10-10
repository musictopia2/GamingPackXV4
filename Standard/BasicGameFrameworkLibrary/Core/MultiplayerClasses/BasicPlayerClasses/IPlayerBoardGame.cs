namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;

public interface IPlayerBoardGame<E> :
    IPlayerColors, IPlayerWithColor<E>
    where E : IFastEnumColorSimple
{
}