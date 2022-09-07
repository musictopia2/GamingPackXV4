namespace BasicGameFrameworkLibrary.Core.GamePieceModels;
/// <summary>
/// this is intended for crossplatform for pickers.  since the part that varies is the enumvalue.  can even be used from things like suit pickers
/// decided to try using the new enums.
/// </summary>
/// <typeparam name="E"></typeparam>
public class BasicPickerData<E> //i think this may have to go to a different namespace.  will do for next version.
   where E : IFastEnumSimple
{
    public E? EnumValue { get; set; } //if color, may still work because of generics (?)
    public bool IsEnabled { get; set; }
    public bool IsSelected { get; set; }
}