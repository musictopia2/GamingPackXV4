namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.CheckersChessHelpers;
public class CheckerChessPieceCP<E> : BasicPickerData<E>
    where E : IFastEnumColorSimple
{
    public bool Highlighted { get; set; } //only checkers need this.  but might as well have here anyways.
}