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
    [LabelColumn]
    public string TeamMate { get; set; } = "None"; //the players name.  only for ui.
    public Spades4PlayerVMData(CommandContainer command,
            SpadesTrickAreaCP trickArea1,
            IGamePackageResolver resolver
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        Bid1 = new NumberPicker(command, resolver);
        Bid1.LoadNormalNumberRangeValues(0, 13);
        Bid1.ChangedNumberValueAsync = Bid1_ChangedNumberValueAsync;
    }
    private Task Bid1_ChangedNumberValueAsync(int chosen)
    {
        BidAmount = chosen;
        return Task.CompletedTask;
    }
    public int BidAmount { get; set; } = -1;
    public NumberPicker Bid1;
    public SpadesTrickAreaCP TrickArea1 { get; set; }
    public DeckObservablePile<Spades4PlayerCardInformation> Deck1 { get; set; }
    public SingleObservablePile<Spades4PlayerCardInformation> Pile1 { get; set; }
    public HandObservable<Spades4PlayerCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<Spades4PlayerCardInformation>? OtherPile { get; set; }
    BasicTrickAreaObservable<EnumSuitList, Spades4PlayerCardInformation> ITrickCardGamesData<Spades4PlayerCardInformation, EnumSuitList>.TrickArea1
    {
        get => TrickArea1;
        set => TrickArea1 = (SpadesTrickAreaCP)value;
    }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}