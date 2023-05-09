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
    [LabelColumn]
    public int RoundNumber { get; set; }
    [LabelColumn]
    public string PassedPlayer { get; set; } = "None"; //only needs to show up when the status is passing.  since its part of the component, then if not passing, would show none.
    //maybe not needed to show game status on vmdata (?)
    public HeartsVMData(CommandContainer command,
            HeartsTrickAreaCP trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
    }
    public HeartsTrickAreaCP TrickArea1 { get; set; }
    public DeckObservablePile<HeartsCardInformation> Deck1 { get; set; }
    public SingleObservablePile<HeartsCardInformation> Pile1 { get; set; }
    public HandObservable<HeartsCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<HeartsCardInformation>? OtherPile { get; set; }
    BasicTrickAreaObservable<EnumSuitList, HeartsCardInformation> ITrickCardGamesData<HeartsCardInformation, EnumSuitList>.TrickArea1
    {
        get => TrickArea1;
        set => TrickArea1 = (HeartsTrickAreaCP)value;
    }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}