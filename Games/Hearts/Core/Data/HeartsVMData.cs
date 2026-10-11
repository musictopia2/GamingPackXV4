namespace Hearts.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class HeartsVMData(CommandContainer command,
        HeartsTrickAreaCP trickArea1
            ) : ITrickCardGamesData<HeartsCardInformation, EnumSuitList>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial EnumSuitList TrumpSuit { get; set; }
    [LabelColumn]
    public partial int RoundNumber { get; set; }
    [LabelColumn]
    public partial string PassedPlayer { get; set; } = "None"; //only needs to show up when the status is passing.  since its part of the component, then if not passing, would show none.
    public HeartsTrickAreaCP TrickArea1 { get; set; } = trickArea1;
    public DeckObservablePile<HeartsCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<HeartsCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<HeartsCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<HeartsCardInformation>? OtherPile { get; set; }
    BasicTrickAreaObservable<EnumSuitList, HeartsCardInformation> ITrickCardGamesData<HeartsCardInformation, EnumSuitList>.TrickArea1
    {
        get => TrickArea1;
        set => TrickArea1 = (HeartsTrickAreaCP)value;
    }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}