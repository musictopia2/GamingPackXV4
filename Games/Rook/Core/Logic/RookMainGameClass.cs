using CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;

namespace Rook.Core.Logic;
[SingletonGame]
public class RookMainGameClass
    : TrickGameClass<EnumColorTypes, RookCardInformation, RookPlayerItem, RookSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly RookVMData _model;
    private readonly CommandContainer _command;
    private readonly RookGameContainer _gameContainer;
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IBidProcesses _bidProcesses;
    private readonly ITrumpProcesses _trumpProcesses;
    private readonly INestProcesses _nestProcesses;
    public RookMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        RookVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<RookCardInformation> cardInfo,
        CommandContainer command,
        RookGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        IBidProcesses bidProcesses,
        ITrumpProcesses trumpProcesses,
        INestProcesses nestProcesses,
        ISystemError error,
        IToast toast,
        RookDelegates delegates
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _aTrick = aTrick;
        GlobalClass.Container = _gameContainer;
        _bidProcesses = bidProcesses;
        _trumpProcesses = trumpProcesses;
        _nestProcesses = nestProcesses;
        _gameContainer.StartNewTrickAsync = StartNewTrickAsync;
        _gameContainer.AfterBidding = FigureOutTemporaryTeams;
        _gameContainer.StartingStatus = () => this.StartingStatus();
        delegates.IsDummy = () =>
        {
            if (PlayerList.Count == 2)
            {
                return true;
            }
            return false;
        };
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        Show4PlayerMates();
        _gameContainer.ShowedOnce = SaveRoot!.GameStatus != EnumStatusList.Bidding;
        SaveRoot.LoadMod(_model!);
        _model!.Dummy1!.HandList.ReplaceRange(SaveRoot.DummyList);
        if (SaveRoot.GameStatus == EnumStatusList.SelectNest)
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
            _model.Status = "Choose the 5 cards to get rid of";
        }
        else
        {
            _model!.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectOneOnly;
        }
        if (SaveRoot.GameStatus != EnumStatusList.Bidding)
        {
            FigureOutTemporaryTeams(); //i think.
        }
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot!.DummyList = _model!.Dummy1!.HandList.ToRegularDeckDict();
        return base.PopulateSaveRootAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        if (PlayerList.Count == 2)
        {
            _model!.Dummy1!.FirstDummy();
        }
        IsLoaded = true;
    }
    public override async Task ContinueTurnAsync()
    {
        if (_gameContainer.ShowedOnce == false)
        {
            await _bidProcesses.BeginBiddingAsync();
            return;
        }
        await base.ContinueTurnAsync();
    }
    private void FigureOutTemporaryTeams()
    {
        if (PlayerList.Count != 3)
        {
            return;
        }
        var player = PlayerList.GetSelf();
        if (player.Id == SaveRoot.WonSoFar)
        {
            _model.TeamMate = "None";
            return; //nobody on your team.
        }
        foreach (var temp in PlayerList)
        {
            if (temp.Id != SaveRoot.WonSoFar && temp.Id != player.Id)
            {
                _model.TeamMate = temp.NickName;
                return;
            }
        }
    }
    public override bool CanEnableTrickAreas => SaveRoot!.GameStatus == EnumStatusList.Normal;
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.25);
        }
        if (SaveRoot!.GameStatus == EnumStatusList.Bidding)
        {
            await ComputerAI.CardToBidAsync(_model, _bidProcesses);
            if (_model!.BidChosen == -1)
            {
                await _bidProcesses.PassBidAsync();
                return;
            }
            await _bidProcesses.ProcessBidAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumStatusList.ChooseTrump)
        {
            ComputerAI.ColorToCall(_model);
            await _trumpProcesses.ProcessTrumpAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumStatusList.SelectNest)
        {
            await _nestProcesses.ProcessNestAsync(ComputerAI.CardsToRemove(this));
            return;
        }
        await PlayCardAsync(ComputerAI.CardToPlay(this, _model));
    }
    private void AssignTeams()
    {
        if (PlayerList.Count < 4)
        {
            return;
        }
        if (PlayerList.All(items => items.Team > 0))
        {
            return;
        }
        RookPlayerItem player;
        player = PlayerList![1];
        player.Team = 1;
        player = PlayerList[2];
        player.Team = 2;
        player = PlayerList[3];
        player.Team = 1;
        player = PlayerList[4];
        player.Team = 2;
        Show4PlayerMates();
    }
    private void Show4PlayerMates()
    {
        if (PlayerList.Count < 4)
        {
            return; //only 4 player has this
        }
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
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        _gameContainer.ShowedOnce = false;
        LoadControls();
        LoadVM();
        AssignTeams();
        if (isBeginning)
        {
            SaveRoot!.LoadMod(_model!);
        }
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.Pass = false;
            //thisPlayer.IsDummy = false;
            thisPlayer.BidAmount = 0;
            thisPlayer.TricksWon = 0;
        });
        if (PlayerList.Count == 4)
        {
            SaveRoot!.HighestBidder = 65; //has to start with 70 unless we decide to change it.
        }
        else
        {
            SaveRoot!.HighestBidder = 35;
        }
        SaveRoot.WonSoFar = 0;
        SaveRoot.TrumpSuit = EnumColorTypes.None;
        _model!.ColorChosen = EnumColorTypes.None;
        SaveRoot.GameStatus = EnumStatusList.Bidding;
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        if (PlayerList.Count == 2)
        {
            DeckRegularDict<RookCardInformation> dummyList = new();
            12.Times(x =>
            {
                dummyList.Add(_model!.Deck1!.DrawCard());
            });
            _model!.Dummy1!.LoadDummyCards(dummyList, this);
        }
        SaveRoot!.NestList.Clear();
        SaveRoot.CardList.Clear();
        5.Times(x =>
        {
            SaveRoot.NestList.Add(_model!.Deck1!.DrawCard());
        });
        _trumpProcesses.ResetTrumps();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "pass":
                await _bidProcesses.PassBidAsync();
                return;
            case "bid":
                _model!.BidChosen = int.Parse(content);
                await _bidProcesses.ProcessBidAsync();
                return;
            case "colorselected":
                _model!.ColorChosen = await js1.DeserializeObjectAsync<EnumColorTypes>(content);
                await _trumpProcesses.ProcessTrumpAsync();
                return;
            case "nestlist":
                var thisList = await js1.DeserializeObjectAsync<DeckRegularDict<RookCardInformation>>(content);
                await _nestProcesses.ProcessNestAsync(thisList);
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
    protected override async Task ShowHumanCanPlayAsync()
    {
        await base.ShowHumanCanPlayAsync();
        if (SaveRoot.GameStatus == EnumStatusList.Bidding)
        {
            _model.Bid1.ReportCanExecuteChange();
        }
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        if (SaveRoot!.DummyPlay)
        {
            SaveRoot.DummyPlay = false;
            WhoTurn = SaveRoot.WonSoFar;
            await StartNewTurnAsync();
            return;
        }
        if (WhoTurn == SaveRoot.WonSoFar)
        {
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
            return;
        }
        if (PlayerList.Count == 2)
        {
            SaveRoot.DummyPlay = true;
            await StartNewTurnAsync();
            return;
        }
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        SingleInfo = PlayerList.GetWhoPlayer();
        WhoTurn = SingleInfo.Id;
        await StartNewTurnAsync();
    }
    private bool HasTrumpOrBird(DeckRegularDict<RookCardInformation> hand)
    {
        if (hand.Any(x => x.IsBird))
        {
            return true;
        }
        if (hand.Any(x => x.GetSuit.Equals(SaveRoot.TrumpSuit)))
        {
            return true;
        }
        return false;
    }
    public override bool IsValidMove(int deck)
    {
        //may need repeating like the game for rage card game.
        if (PlayerList.Count < 4)
        {
            return base.IsValidMove(deck);
        }
        var thisList = SaveRoot!.TrickList;
        if (thisList.Count == 0)
        {
            return true;
        }

        var cardPlayed = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (cardPlayed.IsBird)
        {
            return true; //you can always play the bird no matter what.  guarantees your team will win this.
        }
        var leadCard = thisList.First();
        if (leadCard.GetSuit.Equals(cardPlayed.GetSuit))
        {
            return true;
        }
        if (leadCard.IsBird && cardPlayed.GetSuit == SaveRoot.TrumpSuit)
        {
            return true; //because if you played trump but the bird was led, then its okay.
        }
        DeckRegularDict<RookCardInformation> currentHand;
        currentHand = SingleInfo!.MainHandList;
        if (leadCard.IsBird)
        {
            if (HasTrumpOrBird(currentHand))
            {
                return false; //if the bird is led, then a player must play trump if they have it.
            }
        }
        if (leadCard.GetSuit == SaveRoot.TrumpSuit)
        {
            if (HasTrumpOrBird(currentHand))
            {
                return false; //because if somebody led with trump and you have either trump or bird, you have to play something.
            }
        }
        if (currentHand.Any(x => x.GetSuit.Equals(leadCard.GetSuit)))
        {
            return false; //because you have to follow suit
        }
        return true;
    }
    private int WhoWonTrick(DeckRegularDict<RookCardInformation> thisCol, out bool isDummy)
    {
        if (PlayerList.Count == 4)
        {
            var card = thisCol.Where(x => x.IsBird).SingleOrDefault();
            if (card is not null)
            {
                isDummy = false; //i think should always be false
                return card.Player; //the bird always wins.
            }
        }
        var tempCol = thisCol.Where(items => items.Color == SaveRoot!.TrumpSuit).OrderByDescending(items => items.CardValue).ToRegularDeckDict();
        if (tempCol.Count > 0)
        {
            isDummy = tempCol.First().IsDummy;
            return tempCol.First().Player;
        }
        var thisCard = thisCol.First();
        var leadColor = thisCard.Color;
        tempCol = thisCol.Where(items => items.Color == leadColor).OrderByDescending(items => items.CardValue).ToRegularDeckDict();
        isDummy = tempCol.First().IsDummy;
        return tempCol.First().Player;
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList, out bool dummys);
        RookPlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        await _model.TrickArea1!.AnimateWinAsync(wins, dummys);
        trickList.ForEach(thisCard =>
        {
            RookCardInformation newCard = new();
            newCard.Populate(thisCard.Deck);
            newCard.Player = wins;
            newCard.IsDummy = thisCard.IsDummy;
            SaveRoot.CardList.Add(newCard);
        });
        if (SingleInfo!.MainHandList.Count == 0) //no longer do we have validation for nest cards.
        {
            SaveRoot.NestList.ForEach(thisCard =>
            {
                thisCard.Player = wins;
                thisCard.IsDummy = false;
                SaveRoot.CardList.Add(thisCard);
            });
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
        await StartNewTurnAsync();
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalScore = 0;
            thisPlayer.CurrentScore = 0;
        });
        return Task.CompletedTask;
    }
    private int CalculatePoints(RookPlayerItem player)
    {
        if (PlayerList.Count < 4)
        {
            return SaveRoot!.CardList.Where(items => items.Player == SaveRoot.WonSoFar & player.Id == SaveRoot.WonSoFar | items.Player != SaveRoot.WonSoFar & player.Id != SaveRoot.WonSoFar).Sum(items => items.Points);
        }
        int output = 0;
        foreach (var card in SaveRoot.CardList)
        {
            var won = PlayerList[card.Player]; //i think
            if (won.Team == player.Team)
            {
                output += card.Points;
            }
        }
        return output;
        //return 0; //for now, nobody gets any points.  has to figure out teams first.
    }
    private void Scoring()
    {
        RookPlayerItem winBid = PlayerList[SaveRoot.WonSoFar];
        PlayerList!.ForEach(thisPlayer =>
        {
            int points = CalculatePoints(thisPlayer);
            if (PlayerList.Count < 4)
            {
                if (thisPlayer.Id == SaveRoot!.WonSoFar && points < SaveRoot.HighestBidder)
                {
                    points = SaveRoot.HighestBidder * -1;
                }
            }
            else
            {
                if (thisPlayer.Team == winBid.Team && points < SaveRoot.HighestBidder)
                {
                    points = SaveRoot.HighestBidder * -1;
                }
            }
            thisPlayer.CurrentScore = points;
            thisPlayer.TotalScore += points;
        });
    }
    private bool CanEndGame()
    {
        if (PlayerList.Any(items => items.TotalScore >= 300))
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
            return true;
        }
        return false;
    }
    public override async Task EndRoundAsync()
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.CurrentScore = 0);
        Scoring();
        if (CanEndGame())
        {
            if (PlayerList.Count < 4)
            {
                await ShowWinAsync();
            }
            else
            {
                //4 player win info. we do know the team number though who won.
                int teamWon = SingleInfo!.Team;
                StrCat cats = new();
                PlayerList!.ForConditionalItems(thisplayer => thisplayer.Team == teamWon, thisPlayer =>
                {
                    cats.AddToString(thisPlayer.NickName, ",");
                });
                await ShowWinAsync(cats.GetInfo());
                _command.UpdateAll(); //may fix the problem with not showing the ui for the win.
            }
            return;
        }
        await this.RoundOverNextAsync();
    }
}