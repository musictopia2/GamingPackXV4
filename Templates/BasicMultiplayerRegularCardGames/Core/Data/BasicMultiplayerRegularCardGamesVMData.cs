namespace BasicMultiplayerRegularCardGames.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class BasicMultiplayerRegularCardGamesVMData(CommandContainer command) : IBasicCardGamesData<RegularSimpleCard>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; } = new DeckObservablePile<RegularSimpleCard>(command);
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; } = new SingleObservablePile<RegularSimpleCard>(command);
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; } = new HandObservable<RegularSimpleCard>(command);
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}