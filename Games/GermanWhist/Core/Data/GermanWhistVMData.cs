namespace GermanWhist.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class GermanWhistVMData : ITrickCardGamesData<GermanWhistCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    public GermanWhistVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, GermanWhistCardInformation> trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
    }
    public BasicTrickAreaObservable<EnumSuitList, GermanWhistCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<GermanWhistCardInformation> Deck1 { get; set; }
    public SingleObservablePile<GermanWhistCardInformation> Pile1 { get; set; }
    public HandObservable<GermanWhistCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<GermanWhistCardInformation>? OtherPile { get; set; }
}