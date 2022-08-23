namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public interface IPlayerBoardGame<E> : IPlayerColors
    where E : IFastEnumColorSimple
{
    E? Color { get; set; }
}