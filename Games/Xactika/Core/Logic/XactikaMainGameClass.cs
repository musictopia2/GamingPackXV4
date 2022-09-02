namespace Xactika.Core.Logic;
[SingletonGame]
public class XactikaMainGameClass
    : TrickGameClass<EnumShapes, XactikaCardInformation, XactikaPlayerItem, XactikaSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly XactikaVMData _model;
    private readonly CommandContainer _command;
    private readonly XactikaGameContainer _gameContainer;
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IBidProcesses _bidProcesses;
    private readonly IShapeProcesses _shapeProcesses;
    private readonly IModeProcesses _modeProcesses;
    public XactikaMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        XactikaVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<XactikaCardInformation> cardInfo,
        CommandContainer command,
        XactikaGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        IBidProcesses bidProcesses,
        IShapeProcesses shapeProcesses,
        IModeProcesses modeProcesses,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _aTrick = aTrick;
        _bidProcesses = bidProcesses;
        _shapeProcesses = shapeProcesses;
        _modeProcesses = modeProcesses;
        _gameContainer.StartNewTrickAsync = StartNewTrickAsync;
        _gameContainer.ShowHumanCanPlayAsync = ShowHumanCanPlayAsync;
        _gameContainer.ShowTurn = () => this.ShowTurn();
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        _gameContainer.ShowedOnce = false;
        SaveRoot!.LoadMod(_model!);
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true;
    }
    protected override async Task ComputerTurnAsync()
    {
        if (SaveRoot!.GameStatus == EnumStatusList.ChooseGameType)
        {
            return;
        }
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        if (SaveRoot.GameStatus == EnumStatusList.Bidding)
        {
            int bids = ComputerAI.HowManyToBid(_model);
            _model!.BidChosen = bids;
            await _bidProcesses.ProcessBidAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumStatusList.CallShape)
        {
            await _shapeProcesses.ShapeChosenAsync(ComputerAI.GetShapeChosen());
            return;
        }
        await PlayCardAsync(ComputerAI.CardToPlay(this));
    }
    public override async Task ContinueTrickAsync()
    {
        if (SaveRoot!.ShapeChosen == EnumShapes.None)
        {
            SaveRoot.GameStatus = EnumStatusList.CallShape;
            await _shapeProcesses.FirstCallShapeAsync();
            return;
        }
        await base.ContinueTrickAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        LoadVM();
        _gameContainer.ShowedOnce = false;
        if (isBeginning)
        {
            SaveRoot!.GameStatus = EnumStatusList.ChooseGameType;
            SaveRoot.GameMode = EnumGameMode.None;
            SaveRoot.RoundNumber = 1;
        }
        else if (SaveRoot!.GameMode == EnumGameMode.ToBid)
        {
            SaveRoot.GameStatus = EnumStatusList.Bidding;
        }
        else if (SaveRoot.GameMode != EnumGameMode.None)
        {
            SaveRoot.GameStatus = EnumStatusList.Normal;
            SaveRoot.ClearTricks = true;
        }
        SaveRoot.LoadMod(_model!);
        SaveRoot.ShapeChosen = EnumShapes.None;
        SaveRoot.Value = 0;
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TricksWon = 0;
            thisPlayer.BidAmount = -1;
        });
        return base.StartSetUpAsync(isBeginning);
    }
    public override async Task ContinueTurnAsync()
    {
        if (_gameContainer.ShowedOnce == false)
        {
            await BeginningProcessesAsync();
            return;
        }
        await base.ContinueTurnAsync();
    }
    private async Task BeginningProcessesAsync()
    {
        _gameContainer.ShowedOnce = true;
        if (SaveRoot!.GameMode == EnumGameMode.None)
        {
            await _modeProcesses.EnableOptionsAsync();
            return;
        }
        await _modeProcesses.ProcessGameOptionChosenAsync(SaveRoot.GameMode, false);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "bid":
                _model.BidChosen = int.Parse(content);
                await _bidProcesses.ProcessBidAsync();
                return;
            case "shapeused":
                EnumShapes thisShape = await js.DeserializeObjectAsync<EnumShapes>(content);
                await _shapeProcesses.ShapeChosenAsync(thisShape);
                return;
            case "gameoptionchosen":
                EnumGameMode thisMode = await js.DeserializeObjectAsync<EnumGameMode>(content);
                await _modeProcesses.ProcessGameOptionChosenAsync(thisMode, true);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        this.ShowTurn();
        await SaveStateAsync();
        await ContinueTurnAsync();
    }
    private int WhoWonTrick(DeckRegularDict<XactikaCardInformation> thisCol)
    {
        XactikaCardInformation card;
        if (SaveRoot.ShapeChosen == EnumShapes.Balls)
        {
            card = thisCol.Where(items => items.HowManyBalls == SaveRoot.Value).OrderByDescending(items => items.Value).ThenByDescending(items => items.OrderPlayed).First();
        }
        else if (SaveRoot.ShapeChosen == EnumShapes.Cubes)
        {
            card = thisCol.Where(items => items.HowManyCubes == SaveRoot.Value).OrderByDescending(items => items.Value).ThenByDescending(items => items.OrderPlayed).First();
        }
        else if (SaveRoot.ShapeChosen == EnumShapes.Cones)
        {
            card = thisCol.Where(items => items.HowManyCones == SaveRoot.Value).OrderByDescending(items => items.Value).ThenByDescending(items => items.OrderPlayed).First();
        }
        else if (SaveRoot.ShapeChosen == EnumShapes.Stars)
        {
            card = thisCol.Where(items => items.HowManyStars == SaveRoot.Value).OrderByDescending(items => items.Value).ThenByDescending(items => items.OrderPlayed).First();
        }
        else
        {
            throw new CustomBasicException("Cannot figure out what the card should be");
        }
        return card.Player;
    }
    public override bool CanMakeMainOptionsVisibleAtBeginning => false;
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        XactikaPlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        await _aTrick!.AnimateWinAsync(wins);
        if (SingleInfo!.MainHandList.Count == 0)
        {
            await EndRoundAsync();
            return;
        }
        _model!.PlayerHand1!.EndTurn();
        WhoTurn = wins;
        await StartNewTrickAsync();
    }
    private async Task StartNewTrickAsync()
    {
        _aTrick!.ClearBoard();
        _command!.ManuelFinish = true; //because it could be somebody else's turn.
        _model!.ShapeChoose1!.Reset();
        SaveRoot!.ShapeChosen = EnumShapes.None;
        SaveRoot.Value = 0;
        await StartNewTurnAsync();
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalScore = 0;
            thisPlayer.CurrentScore = 0;
        });
        SaveRoot!.GameStatus = EnumStatusList.ChooseGameType;
        SaveRoot.RoundNumber = 1;
        SaveRoot.GameMode = EnumGameMode.None;
        return Task.CompletedTask;
    }
    public override bool IsValidMove(int deck)
    {
        if (SaveRoot!.ShapeChosen == EnumShapes.None)
        {
            return true;
        }
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (SaveRoot.ShapeChosen == EnumShapes.Balls)
        {
            if (thisCard.HowManyBalls == SaveRoot.Value)
            {
                return true;
            }
            return !SingleInfo!.MainHandList.Any(items => items.HowManyBalls == SaveRoot.Value);
        }
        if (SaveRoot.ShapeChosen == EnumShapes.Cones)
        {
            if (thisCard.HowManyCones == SaveRoot.Value)
            {
                return true;
            }
            return !SingleInfo!.MainHandList.Any(items => items.HowManyCones == SaveRoot.Value);
        }
        if (SaveRoot.ShapeChosen == EnumShapes.Cubes)
        {
            if (thisCard.HowManyCubes == SaveRoot.Value)
            {
                return true;
            }
            return !SingleInfo!.MainHandList.Any(items => items.HowManyCubes == SaveRoot.Value);
        }
        if (SaveRoot.ShapeChosen == EnumShapes.Stars)
        {
            if (thisCard.HowManyStars == SaveRoot.Value)
            {
                return true;
            }
            return !SingleInfo!.MainHandList.Any(items => items.HowManyStars == SaveRoot.Value);
        }
        throw new CustomBasicException("Cannot figure out whether the card can be played or not");
    }
    private static int CalculateScore(int bidAmount, int amountWon)
    {
        if (bidAmount == amountWon)
        {
            return bidAmount;
        }
        if (bidAmount > amountWon)
        {
            return amountWon - bidAmount;
        }
        else
        {
            return bidAmount - amountWon;
        }
    }
    public override async Task EndRoundAsync()
    {
        if (SaveRoot!.GameMode == EnumGameMode.ToBid)
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                var bidAmount = thisPlayer.BidAmount;
                var amountWon = thisPlayer.TricksWon;
                thisPlayer.CurrentScore = CalculateScore(bidAmount, amountWon);
                thisPlayer.TotalScore += thisPlayer.CurrentScore;
            });
            if (SaveRoot.RoundNumber == 8 || Test!.ImmediatelyEndGame)
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
                await ShowWinAsync();
                return;
            }
            SaveRoot.RoundNumber++;
            await this.RoundOverNextAsync();
            return;
        }
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentScore = thisPlayer.TricksWon;
            thisPlayer.TotalScore += thisPlayer.CurrentScore;
        });
        if (SaveRoot.RoundNumber == 8 || Test!.ImmediatelyEndGame)
        {
            if (SaveRoot.GameMode == EnumGameMode.ToWin)
            {
                SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
            }
            else
            {
                SingleInfo = PlayerList.OrderBy(items => items.TotalScore).First();
            }
            await ShowWinAsync();
            return;
        }
        SaveRoot.RoundNumber++;
        await this.RoundOverNextAsync();
    }
}