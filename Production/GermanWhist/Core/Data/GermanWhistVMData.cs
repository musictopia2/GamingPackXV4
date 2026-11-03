namespace GermanWhist.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class GermanWhistVMData(CommandContainer command,
        BasicTrickAreaObservable<EnumSuitList, GermanWhistCardInformation> trickArea1
            ) : ITrickCardGamesData<GermanWhistCardInformation, EnumSuitList>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial EnumSuitList TrumpSuit { get; set; }
    public BasicTrickAreaObservable<EnumSuitList, GermanWhistCardInformation> TrickArea1 { get; set; } = trickArea1;
    public DeckObservablePile<GermanWhistCardInformation> Deck1 { get; set; } = new(command);
    public SingleObservablePile<GermanWhistCardInformation> Pile1 { get; set; } = new(command);
    public HandObservable<GermanWhistCardInformation> PlayerHand1 { get; set; } = new(command);
    public SingleObservablePile<GermanWhistCardInformation>? OtherPile { get; set; }
}