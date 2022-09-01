namespace Fluxx.Core.Containers;
[SingletonGame]
[AutoReset]
[UseLabelGrid]
public partial class FluxxVMData : IBasicCardGamesData<FluxxCardInformation>
{
    public FluxxVMData(CommandContainer command, TestOptions test, IAsyncDelayer delayer)
    {
        Deck1 = new (command);
        Pile1 = new (command);
        PlayerHand1 = new (command);
        Keeper1 = new (command);
        Goal1 = new (command);
        Goal1.Text = "Goal Cards";
        Goal1.Maximum = 3;
        Goal1.AutoSelect = EnumHandAutoType.SelectOneOnly;
        Keeper1.AutoSelect = EnumHandAutoType.SelectAsMany;
        Keeper1.Text = "Your Keepers";
        CardDetail = new DetailCardObservable();
        _test = test;
        _delayer = delayer;
    }
    public DetailCardObservable CardDetail;
    public HandObservable<KeeperCard> Keeper1;
    public HandObservable<GoalCard> Goal1;
    public DeckObservablePile<FluxxCardInformation> Deck1 { get; set; }
    public SingleObservablePile<FluxxCardInformation> Pile1 { get; set; }
    public HandObservable<FluxxCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<FluxxCardInformation>? OtherPile { get; set; }
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int PlaysLeft { get; set; }
    [LabelColumn]
    public int HandLimit { get; set; }
    [LabelColumn]
    public int KeeperLimit { get; set; }
    [LabelColumn]
    public int PlayLimit { get; set; }
    [LabelColumn]
    public bool AnotherTurn { get; set; }
    [LabelColumn]
    public int DrawBonus { get; set; }
    [LabelColumn]
    public int PlayBonus { get; set; }
    [LabelColumn]
    public int CardsDrawn { get; set; }
    [LabelColumn]
    public int DrawRules { get; set; }
    [LabelColumn]
    public int PreviousBonus { get; set; }
    [LabelColumn]
    public int CardsPlayed { get; set; }
    [LabelColumn]
    public string OtherTurn { get; set; } = "";
    private readonly TestOptions _test;
    private readonly IAsyncDelayer _delayer;
    public async Task ShowPlayCardAsync(FluxxCardInformation card)
    {
        if (card.Deck != CardDetail!.CurrentCard.Deck)
        {
            CardDetail.ShowCard(card);
            if (_test.NoAnimations == false)
            {
                await _delayer.DelaySeconds(1);
            }
        }
        CardDetail.ResetCard();
    }
    internal void UnselectAllCards()
    {
        PlayerHand1!.UnselectAllObjects();
        Keeper1!.UnselectAllObjects();
        Goal1!.UnselectAllObjects();
    }
}