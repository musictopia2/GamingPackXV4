namespace Hearts.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class HeartsVMData : ITrickCardGamesData<HeartsCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    public HeartsVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, HeartsCardInformation> trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
    }
    public BasicTrickAreaObservable<EnumSuitList, HeartsCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<HeartsCardInformation> Deck1 { get; set; }
    public SingleObservablePile<HeartsCardInformation> Pile1 { get; set; }
    public HandObservable<HeartsCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<HeartsCardInformation>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}