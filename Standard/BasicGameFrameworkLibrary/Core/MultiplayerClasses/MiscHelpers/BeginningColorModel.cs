namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MiscHelpers;
public class BeginningColorModel<E, P>(CommandContainer command) : IBeginningColorModel<E>
    where E : struct, IFastEnumColorList<E>
    where P : class, IPlayerBoardGame<E>, new()
{
    public BoardGamesColorPicker<E, P> ColorChooser { get; set; } = new BoardGamesColorPicker<E, P>(command, new ColorListChooser<E>());
    SimpleEnumPickerVM<E> IBeginningColorModel<E>.ColorChooser => ColorChooser;
}