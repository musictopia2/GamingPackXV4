namespace Spades2Player.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class Spades2PlayerVMData : ITrickCardGamesData<Spades2PlayerCardInformation, EnumSuitList>
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
    public partial EnumGameStatus GameStatus { get; set; }
    partial void OnGameStatusChanged(EnumGameStatus value)
    {
        ChangeScreen?.Invoke();
    }
    public int BidAmount { get; set; } = -1;
    public Spades2PlayerVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, Spades2PlayerCardInformation> trickArea1,
            IGamePackageResolver resolver
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        OtherPile = new SingleObservablePile<Spades2PlayerCardInformation>(command);
        Bid1 = new NumberPicker(command, resolver);
        Bid1.LoadNormalNumberRangeValues(0, 13);
        OtherPile.Text = "Current";
        OtherPile.Visible = false;
    }
    public Action? ChangeScreen { get; set; }
    public NumberPicker Bid1;
    public BasicTrickAreaObservable<EnumSuitList, Spades2PlayerCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<Spades2PlayerCardInformation> Deck1 { get; set; }
    public SingleObservablePile<Spades2PlayerCardInformation> Pile1 { get; set; }
    public HandObservable<Spades2PlayerCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<Spades2PlayerCardInformation>? OtherPile { get; set; }
}