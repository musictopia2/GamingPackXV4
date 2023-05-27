namespace Spades4Player.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class Spades4PlayerVMData : ITrickCardGamesData<Spades4PlayerCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    public Spades4PlayerVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, Spades4PlayerCardInformation> trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
    }
    public BasicTrickAreaObservable<EnumSuitList, Spades4PlayerCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<Spades4PlayerCardInformation> Deck1 { get; set; }
    public SingleObservablePile<Spades4PlayerCardInformation> Pile1 { get; set; }
    public HandObservable<Spades4PlayerCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<Spades4PlayerCardInformation>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}