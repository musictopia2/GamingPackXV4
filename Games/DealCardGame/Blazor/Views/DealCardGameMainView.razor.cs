namespace DealCardGame.Blazor.Views;
public partial class DealCardGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private DealCardGameVMData? _vmData;
    private DealCardGameGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<DealCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<DealCardGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DealCardGameVMData.NormalTurn))
            .AddLabel("Status", nameof(DealCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(DealCardGamePlayerItem.ObjectCount))
            .AddColumn("Money", true, nameof(DealCardGamePlayerItem.Money), category: EnumScoreSpecialCategory.Currency)

            ; //cards left is common.  can be anything you need.
        base.OnInitialized();
    }
    private BasicList<DealCardGameCardInformation> GetCards(int item)
    {
        BasicList<DealCardGameCardInformation> output = [];
        var firsts = _gameContainer!.DeckList.First(x => x.MainColor == EnumColor.FromValue(item) && x.CardType == EnumCardType.PropertyRegular);
        4.Times(() =>
        {
            DealCardGameCardInformation card = new();
            card.Populate(firsts.Deck);
            output.Add(card);
            //DealCardGameCardInformation card = new();
            //int index = 20 + item;
            //card.Populate(index);
            //output.Add(card);
        });
        return output;
    }
    private BasicList<DealCardGamePlayerItem> GetPlayers()
    {
        var output = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
        return output;
    }
    private static string Rows => $"{gg1.RepeatMinimum(2)}";
    private BasicGameCommand PlayCommand => DataContext!.PlayCommand!;
    private BasicGameCommand EndTurnCommand => DataContext!.EndTurnCommand!;
    private BasicGameCommand BankCommand => DataContext!.BankCommand!;
    private BasicGameCommand SetChosenCommand => DataContext!.SetChosenCommand!;
}