namespace Uno.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class UnoVMData : IBasicCardGamesData<UnoCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string NextPlayer { get; set; } = "";
    public UnoVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        ColorPicker = new(command, new ColorListChooser<EnumColorTypes>());
        Stops = new();
        Stops.MaxTime = 3000;
        ColorPicker.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
    }
    public DeckObservablePile<UnoCardInformation> Deck1 { get; set; }
    public SingleObservablePile<UnoCardInformation> Pile1 { get; set; }
    public HandObservable<UnoCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<UnoCardInformation>? OtherPile { get; set; }
    public SimpleEnumPickerVM<EnumColorTypes> ColorPicker;
    public CustomStopWatchCP Stops;
}