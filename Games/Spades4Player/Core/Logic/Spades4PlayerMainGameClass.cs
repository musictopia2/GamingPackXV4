namespace Spades4Player.Core.Logic;
[SingletonGame]
public class Spades4PlayerMainGameClass
    : TrickGameClass<EnumSuitList, Spades4PlayerCardInformation, Spades4PlayerPlayerItem, Spades4PlayerSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly Spades4PlayerVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly Spades4PlayerGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly IAdvancedTrickProcesses _aTrick;
    public Spades4PlayerMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        Spades4PlayerVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<Spades4PlayerCardInformation> cardInfo,
        CommandContainer command,
        Spades4PlayerGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _aTrick = aTrick;
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        ShowTeamMates();
        //anything else needed is here.
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
    }
    private void ShowTeamMates()
    {
        var player = PlayerList.GetSelf(); //for each person's ui.
        foreach (var item in PlayerList)
        {
            if (item.Team == player.Team && item.Id != player.Id)
            {
                _model.TeamMate = item.NickName;
                return;
            }
        }
    }
    private void AssignTeams()
    {
        if (PlayerList.All(items => items.Team > 0))
        {
            return;
        }
        Spades4PlayerPlayerItem player;
        player = PlayerList![1];
        player.Team = 1;
        player = PlayerList[2];
        player.Team = 2;
        player = PlayerList[3];
        player.Team = 1;
        player = PlayerList[4];
        player.Team = 2;
        ShowTeamMates();
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
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.25);
        }
        if (SaveRoot.GameStatus == EnumGameStatus.Bidding)
        {
            _model!.BidAmount = _model!.Bid1!.NumberToChoose();
            if (SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
            {
                await _gameContainer.Network!.SendAllAsync("bid", _model.BidAmount);
            }
            await ProcessBidAsync();
            return;
        }
        var moveList = SingleInfo!.MainHandList.Where(xx => IsValidMove(xx.Deck)).Select(xx => xx.Deck).ToBasicList();
        int deck = moveList.First();
        if (BasicData!.MultiPlayer)
        {
            await Network!.SendAllAsync("trickplay", deck);
        }
        await PlayCardAsync(deck);
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        LoadVM();
        AssignTeams();
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TricksWon = 0;
            thisPlayer.HowManyBids = -1;
        });
        SaveRoot.GameStatus = EnumGameStatus.Bidding;
        return base.StartSetUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.
            case "bid":
                _model!.BidAmount = int.Parse(content);
                await ProcessBidAsync();
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public async Task ProcessBidAsync()
    {
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            _model!.Bid1!.SelectNumberValue(_model.BidAmount);
            _command.UpdateAll();
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.5);
            }
            SingleInfo.HowManyBids = _model.BidAmount; //i think.
            _model.Bid1.UnselectAll();
        }
        SingleInfo.HowManyBids = _model!.BidAmount;
        if (PlayerList.All(Items => Items.HowManyBids > -1))
        {
            SaveRoot!.GameStatus = EnumGameStatus.Normal;
            _aTrick!.ClearBoard();
        }
        await EndTurnAsync();
    }
    private bool CanEndGame()
    {
        if (PlayerList.Any(items => items.TotalScore >= 500))
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
            return true;
        }
        return false;
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model!.PlayerHand1!.EndTurn(); //i think
        }
        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public override async Task EndTurnAsync() //may be fine (?)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.

        //anything else is here.  varies by game.


        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    private static int WhoWonTrick(DeckRegularDict<Spades4PlayerCardInformation> thisCol) //done i think.
    {
        var tempCol = thisCol.Where(items => items.Suit == EnumSuitList.Spades).OrderByDescending(items => items.Value).ToRegularDeckDict();
        if (tempCol.Count > 0)
        {
            return tempCol.First().Player;
        }
        var leadCard = thisCol.First();
        var highCard = thisCol.Where(x => x.Suit == leadCard.Suit).OrderByDescending(x => x.Value).First();
        return highCard.Player; //try this (?)
    }
    public override async Task EndTrickAsync() //done.
    {
        var trickList = SaveRoot!.TrickList;
        if (trickList.Any(Items => Items.Suit == EnumSuitList.Spades))
        {
            SaveRoot.TrickStatus = EnumTrickStatus.SuitBroken;
        }
        else if (SaveRoot.TrickStatus == EnumTrickStatus.FirstTrick)
        {
            SaveRoot.TrickStatus = EnumTrickStatus.NoSuit;
        }
        int wins = WhoWonTrick(trickList);
        Spades4PlayerPlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        await _aTrick!.AnimateWinAsync(wins);
        if (SingleInfo!.MainHandList.Count == 0)
        {
            await EndRoundAsync();
            return; //most of the time its in rounds.
        }
        _model!.PlayerHand1!.EndTurn();
        WhoTurn = wins; //most of the time, whoever wins leads again.
        await StartNewTrickAsync();
    }
    private async Task StartNewTrickAsync() //done i think.
    {
        _aTrick!.ClearBoard();
        _command!.ManuelFinish = true; //because it could be somebody else's turn.
        await StartNewTurnAsync(); //hopefully this simple.
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalScore = 0;
            thisPlayer.Bags = 0;
            thisPlayer.CurrentScore = 0;
        });
        return Task.CompletedTask;
    }
    public override async Task EndRoundAsync()
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.CurrentScore = 0);
        Scoring();
        bool doEnd = CanEndGame();
        if (doEnd)
        {
            int teamWon = SingleInfo!.Team;
            StrCat cats = new();
            PlayerList!.ForConditionalItems(thisplayer => thisplayer.Team == teamWon, thisPlayer =>
            {
                cats.AddToString(thisPlayer.NickName, ",");
            });
            await ShowWinAsync(cats.GetInfo());
            _command.UpdateAll(); //may fix the problem with not showing the ui for the win.
            return;
        }
        await this.RoundOverNextAsync();
    }
    private void Scoring()
    {
        //this is all the rules for scoring.
        2.Times(x =>
        {
            BasicList<Spades4PlayerPlayerItem> list = PlayerList.Where(y => y.Team == x).ToBasicList();
            int nilScore = GetNilScore(list);
            int newBags = GetBagsInRound(list);
            int bidScore = BidScore(list);
            int bagPenalty = BagPenalty(list, newBags);
            int totalBags = TotalBagsInGame(list, newBags, bagPenalty);
            foreach (var item in list)
            {
                int score = nilScore + newBags + bagPenalty + bidScore;
                item.CurrentScore += score;
                item.TotalScore += score;
                item.Bags = totalBags;
            }
        });
    }
    private static int BidScore(BasicList<Spades4PlayerPlayerItem> list)
    {
        int required = list.Sum(x => x.HowManyBids);
        int obtained = list.Sum(x => x.TricksWon);
        if (obtained >= required)
        {
            return required * 10;
        }
        return required * -10;
    }
    private static int GetBagsInRound(BasicList<Spades4PlayerPlayerItem> list)
    {
        int required = list.Sum(x => x.HowManyBids);
        int obtained = list.Sum(x => x.TricksWon);
        if (obtained <= required)
        {
            return 0;
        }
        return obtained - required;
    }
    private static int BagPenalty(BasicList<Spades4PlayerPlayerItem> list, int newBags)
    {
        int bagsSoFar = list.First().Bags;
        int total = bagsSoFar + newBags;
        if (total >= 10)
        {
            return -100;
        }
        return 0;
    }
    private static int TotalBagsInGame(BasicList<Spades4PlayerPlayerItem> list, int newBags, int bagPenalty)
    {
        int bagsSoFar = list.First().Bags;
        bagsSoFar += newBags;
        if (bagPenalty < 0)
        {
            bagsSoFar -= 10;
        }
        return bagsSoFar;
    }
    private static int GetNilScore(BasicList<Spades4PlayerPlayerItem> list)
    {
        int output = 0;
        foreach (var item in list)
        {
            if (item.HowManyBids == 0)
            {
                if (item.TricksWon > 0)
                {
                    output -= 100;
                }
                else
                {
                    output += 100;
                }
            }
        }
        return output;
    }
}