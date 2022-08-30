namespace CoveredUp.Core.Logic;
[SingletonGame]
public class CoveredUpMainGameClass
    : CardGameClass<RegularSimpleCard, CoveredUpPlayerItem, CoveredUpSaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly CoveredUpVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly CoveredUpGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly IToast _toast;
    public CoveredUpMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        CoveredUpVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<RegularSimpleCard> cardInfo,
        CommandContainer command,
        CoveredUpGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
        _gameContainer.PileClickedAsync = CardClickedAsync;
    }
    private async Task CardClickedAsync(RegularSimpleCard card)
    {
        //_toast.ShowInfoToast(card.ToString());
        if (BasicData!.MultiPlayer == true)
        {
            await Network!.SendAllAsync("tradecard", card.Deck);
        }
        await TradeCardAsync(card.Deck);
    }
    private async Task TradeCardAsync(int deck)
    {
        SingleInfo!.PlayerBoard!.TradeCard(deck, _model.OtherPile!.GetCardInfo().Deck);
        RegularSimpleCard tempCard = new();
        tempCard.Populate(deck);
        await DiscardAsync(tempCard);
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        SaveRoot.LoadMod(_model);
        //foreach (var player in PlayerList)
        //{
        //    player.PlayerBoard!.PointsSoFar(); //for testing.

        //}
        //anything else needed is here.
        return base.FinishGetSavedAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true; //i think needs to be here.
    }
    protected override async Task ComputerTurnAsync()
    {
        //if there is nothing, then just won't do anything.

        await Task.CompletedTask;
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        SaveRoot!.Round++;
        SaveRoot.UpTo = 0; //decided will be 0, 1, etc.  until its up to 7.
        SaveRoot.WentOut = false;
        SaveRoot.Begins = 0;
        SaveRoot.LoadMod(_model!);
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        foreach (var player in PlayerList)
        {
            foreach (var card in player.MainHandList)
            {
                card.IsUnknown = true;
            }
            var fins = player.MainHandList[4]; //has to be the fourth one because how the cards are laid out.
            fins.IsUnknown = false;
            player.PointsSoFar = fins.Points();
        }
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.
            case "tradecard":
                await TradeCardAsync(int.Parse(content));
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();

        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public override Task PopulateSaveRootAsync()
    {
        PlayerList.ForEach(player =>
        {
            if (player.PlayerBoard != null)
            {
                player.MainHandList = player.PlayerBoard!.ObjectList.ToRegularDeckDict();
            }
            
        });
        return base.PopulateSaveRootAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
        SingleInfo.PlayerBoard!.EndTurn();
        if (SingleInfo.PlayerBoard.FirstWentOut())
        {
            SaveRoot.Begins = WhoTurn; //i think.
            _toast.ShowInfoToast($"{SingleInfo.NickName} has gone out.  Therefore, everyone gets one last turn");
            SaveRoot.WentOut = true;
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        if (SaveRoot.WentOut && WhoTurn == SaveRoot.Begins)
        {
            await EndRoundAsync();
            return;
        }
        if (WhoTurn == WhoStarts && SaveRoot.UpTo < 6)
        {
            SaveRoot.UpTo++;
        }
        await StartNewTurnAsync();
    }
    public override async Task EndRoundAsync()
    {
        CalculateScore();
        if (SaveRoot!.Round == 10)
        {
            SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    private void CalculateScore()
    {
        foreach (var player in PlayerList)
        {
            player.PreviousScore = player.PlayerBoard!.TotalPointsInRound();
            player.TotalScore += player.PreviousScore;
        }
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.PreviousScore = 0;
            thisPlayer.PointsSoFar = 0;
            thisPlayer.TotalScore = 0;
        });
        SaveRoot!.Round = 0;
        return Task.CompletedTask;
    }
}