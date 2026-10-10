namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MiscHelpers;
public class BeginningColorModel<E, P>(CommandContainer command) : IBeginningColorModel<E>
    where E : struct, IFastEnumColorList<E>
    where P : class, IPlayerWithColor<E>, new()
{
    public PlayerColorPicker<E, P> ColorChooser { get; set; } = new PlayerColorPicker<E, P>(command, new ColorListChooser<E>());
    SimpleEnumPickerVM<E> IBeginningColorModel<E>.ColorChooser => ColorChooser;
}