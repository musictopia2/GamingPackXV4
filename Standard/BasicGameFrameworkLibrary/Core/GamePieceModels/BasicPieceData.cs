namespace BasicGameFrameworkLibrary.Core.GamePieceModels;
/// <summary>
/// this is intended for crossplatform for pickers.  since the part that varies is the enumvalue.  can even be used from things like suit pickers
/// decided to try using the new enums.
/// </summary>
/// <typeparam name="E"></typeparam>
[UseStateChanged]
public partial class BasicPickerData<E> //i think this may have to go to a different namespace.  will do for next version.
   where E : IFastEnumSimple
{

    //only iffy is enumvalue (hopefully the risk pays off to not track this).

    //[StateProperty]
    public E? EnumValue { get; set; } //if color, may still work because of generics (?)
    [StateProperty]
    public partial bool IsEnabled { get; set; }
    [StateProperty]
    public partial bool IsSelected { get; set; }
}