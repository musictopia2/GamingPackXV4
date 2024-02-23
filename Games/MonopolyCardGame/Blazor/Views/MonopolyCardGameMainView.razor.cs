namespace MonopolyCardGame.Blazor.Views;
public partial class MonopolyCardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private MonopolyCardGameVMData? _vmData;
    private MonopolyCardGameGameContainer? _gameContainer;
    private BasicList<MonopolyCardGamePlayerItem> _players = [];
    private MonopolyCardGamePlayerItem? _tradeOpponent;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<MonopolyCardGameVMData>();
        _gameContainer = aa1.Resolver.Resolve<MonopolyCardGameGameContainer>();
        _players = _gameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        _gameContainer.StartCustomTrade = StartTrade;
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(MonopolyCardGameVMData.NormalTurn))
           .AddLabel("Status", nameof(MonopolyCardGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Cards Left", true, nameof(MonopolyCardGamePlayerItem.ObjectCount))
            .AddColumn("Previous Money", true, nameof(MonopolyCardGamePlayerItem.PreviousMoney), category: EnumScoreSpecialCategory.Currency)
            .AddColumn("Total Money", true, nameof(MonopolyCardGamePlayerItem.TotalMoney), category: EnumScoreSpecialCategory.Currency);
        base.OnInitialized();
    }
    private void StartTrade(MonopolyCardGamePlayerItem player)
    {
        _tradeOpponent = player;
    }
    private async Task CancelTradeAsync()
    {
        var player = _gameContainer!.PlayerList!.GetSelf();
        await player.TradePile!.PutBackAsync();
        _tradeOpponent = null;
        await DataContext!.MainGame.ForceAllowPlayAsync();
    }
    private async Task FinishTradeAsync(TradeModel trade)
    {
        await Task.Delay(0);
        _tradeOpponent = null;
        await DataContext!.TradeAsync(trade);
    }
    private ICustomCommand ResumeCommand => DataContext!.ResumeCommand!;
    private ICustomCommand GoOutCommand => DataContext!.GoOutCommand!;
    private ICustomCommand OrganizeCardsCommand => DataContext?.OrganizeCardsCommand!;
    private async Task FinishedOrganizingCardsAsync()
    {
        if (DataContext!.PreviousStatus == EnumWhatStatus.None)
        {
            throw new CustomBasicException("The previous state cannot be none");
        }
        _gameContainer!.SaveRoot.GameStatus = DataContext.PreviousStatus;
        _gameContainer.SaveRoot.ManuelStatus = EnumManuelStatus.None;
        DataContext.PreviousStatus = EnumWhatStatus.None;
        DataContext.MainGame.SortCards();
        await DataContext.MainGame.ForceAllowPlayAsync();
        //DataContext.CheckTradePileStatus();
    }
    private MonopolyCardGamePlayerItem GetPlayer => _gameContainer!.SaveRoot.PlayerList.GetWhoPlayer();
}