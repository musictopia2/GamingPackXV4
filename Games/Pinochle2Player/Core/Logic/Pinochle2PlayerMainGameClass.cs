namespace Pinochle2Player.Core.Logic;
[SingletonGame]
public class Pinochle2PlayerMainGameClass
    : TrickGameClass<EnumSuitList, Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly Pinochle2PlayerVMData _model;
    private readonly CommandContainer _command;
    private readonly Pinochle2PlayerGameContainer _gameContainer;
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IToast _toast;
    private readonly ITrickPlay _trickPlay;
    public Pinochle2PlayerMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        Pinochle2PlayerVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<Pinochle2PlayerCardInformation> cardInfo,
        CommandContainer command,
        Pinochle2PlayerGameContainer gameContainer,
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
        _trickPlay = trickPlay;
        _aTrick = aTrick;
        _toast = toast;
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        SingleInfo = PlayerList.GetSelf();
        SortCards();
        if (SaveRoot!.StartMessage != "")
        {
            _toast.ShowInfoToast(SaveRoot.StartMessage);
        }
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
        if (SaveRoot.ChooseToMeld)
        {
            _aTrick.HideCards();
        }
    }
    protected override async Task LastPartOfSetUpBeforeBindingsAsync()
    {
        LoadControls();
        LoadVM();
        SaveRoot!.MeldList.Clear();
        SaveRoot.CardList.Clear();
        SaveRoot.TrumpSuit = _model.Pile1!.GetCardInfo().Suit;
        _aTrick!.ClearBoard();
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TricksWon = 0;
            thisPlayer.CurrentScore = 0;
        });
        var thisCard = _model.Pile1.GetCardInfo();
        if (thisCard.Value == EnumRegularCardValueList.Nine)
        {
            int player = WhoStarts;
            if (player == 1)
            {
                player = 2;
            }
            else
            {
                player = 1;
            }
            MeldClass thisMeld = new();
            thisMeld.Player = player;
            thisMeld.ClassAValue = EnumClassA.Dix;
            SaveRoot.MeldList.Add(thisMeld);
            var tempPlayer = PlayerList[player];
            SaveRoot.StartMessage = $"{tempPlayer.NickName} gets an exta 10 points because the discard to begin with is a dix and is not going first";
            CalculateScore(player);
            _toast.ShowInfoToast(SaveRoot.StartMessage);
        }
        await base.LastPartOfSetUpBeforeBindingsAsync();
    }
    public override Task PopulateSaveRootAsync()
    {
        return base.PopulateSaveRootAsync();
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
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        if (SaveRoot!.ChooseToMeld)
        {
            await EndTurnAsync(); //because the computer player will never meld cards.
            return;
        }
        var tempList = SingleInfo!.MainHandList.Where(items => IsValidMove(items.Deck)).ToRegularDeckDict();
        await PlayCardAsync(tempList.GetRandomItem().Deck);
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        SaveRoot!.ChooseToMeld = false;
        return base.StartSetUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "meld":
                var tempList = await content.GetSavedIntegerListAsync();
                await MeldAsync(tempList);
                return;
            case "exchangediscard":
                await ExchangeDiscardAsync(int.Parse(content));
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    protected override int PossibleOtherSelected(int firstChosen, out string message)
    {
        message = ""; //this represents the message that will be a messagebox.
        int others;
        others = _model!.YourMelds!.ObjectSelected();
        if (others == 0)
        {
            return firstChosen;
        }
        if (others > 0 && firstChosen > 0)
        {
            message = "You can choose from the melds pile or from hand but not both";
            return 0;
        }
        if (others > 0 || firstChosen > 0)
        {
            message = "You can choose a card from hand";
            return 0;
        }
        int decks = firstChosen;
        if (decks == 0 && others > 0)
        {
            return others;
        }
        return decks;
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        var thisCard = _model.Deck1!.DrawCard();
        thisCard.Drew = true;
        SingleInfo.MainHandList.Add(thisCard);
        int newTurn;
        if (WhoTurn == 1)
        {
            newTurn = 2;
        }
        else
        {
            newTurn = 1;
        }
        var tempPlayer = PlayerList[newTurn];
        if (_model.Deck1.IsEndOfDeck())
        {
            thisCard = _model.Pile1!.GetCardInfo();
            _model.Pile1.ClearCards();
        }
        else
        {
            thisCard = _model.Deck1.DrawCard();
        }
        thisCard.Drew = true;
        tempPlayer.MainHandList.Add(thisCard);
        if (_model.Deck1.IsEndOfDeck())
        {
            MeldsBackInHand();
        }
        SingleInfo = PlayerList.GetSelf();
        SortCards();
        SingleInfo = PlayerList.GetWhoPlayer();
        await StartNewTrickAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        SaveRoot!.ChooseToMeld = false;
        await ContinueTurnAsync();
    }
    public override async Task ContinueTrickAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    private int WhoWonTrick(DeckRegularDict<Pinochle2PlayerCardInformation> thisCol)
    {
        var leadCard = thisCol.First();
        var thisCard = thisCol.Last();
        if (thisCard.Suit == SaveRoot!.TrumpSuit && leadCard.Suit != SaveRoot.TrumpSuit)
        {
            return WhoTurn;
        }
        if (leadCard.Suit == SaveRoot.TrumpSuit && thisCard.Suit != SaveRoot.TrumpSuit)
        {
            return leadCard.Player;
        }
        if (thisCard.Suit == leadCard.Suit)
        {
            if (thisCard.PinochleCardValue > leadCard.PinochleCardValue)
            {
                return WhoTurn;
            }
        }
        return leadCard.Player;
    }
    private void CalculateScore(int player)
    {
        var firstPoints = SaveRoot!.CardList.Where(items => items.Player == player).Sum(items => items.Points);
        var secondPoints = SaveRoot.MeldList.Where(items => items.Player == player).Sum(items => (int)items.ClassAValue + (int)items.ClassBValue + (int)items.ClassCValue);
        var thisPlayer = PlayerList![player];
        thisPlayer.CurrentScore = firstPoints + secondPoints;
    }
    private int CardsLeft()
    {
        var thisPlayer = PlayerList!.GetWhoPlayer();
        if (_model!.Pile1!.PileEmpty() == true)
        {
            return thisPlayer.ObjectCount;
        }
        int counts = SaveRoot!.MeldList.Where(items => items.Player == WhoTurn).Select(items => items.CardList.Count).ToBasicList().Count;
        return counts = thisPlayer.ObjectCount;
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        Pinochle2PlayerPlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        await _aTrick!.AnimateWinAsync(wins);
        trickList.ForEach(card =>
        {
            card.Points = card.PinochleCardValue;
            card.Player = wins;
        });
        _aTrick.HideCards();
        SaveRoot.CardList.AddRange(trickList);
        CalculateScore(wins);
        WhoTurn = wins;
        SingleInfo = PlayerList.GetWhoPlayer();
        int lefts = CardsLeft();
        if (lefts == 0)
        {
            if (_model!.Pile1!.PileEmpty() == false)
            {
                throw new CustomBasicException("Never went to end");
            }
            SingleInfo.CurrentScore += 10;
            _toast.ShowInfoToast($"{SingleInfo.NickName} gets an extra 10 points for winning the last trick");
            await EndRoundAsync();
            return;
        }
        SingleInfo = PlayerList.GetSelf();
        _model!.PlayerHand1!.EndTurn();
        SortCards();
        SingleInfo = PlayerList.GetWhoPlayer();
        if (_model.Pile1!.PileEmpty() == false)
        {
            SaveRoot.ChooseToMeld = true;
            this.ShowTurn();
            await ContinueTurnAsync();
            return;
        }
        await StartNewTrickAsync();
    }
    private async Task StartNewTrickAsync()
    {
        _aTrick!.ClearBoard();
        _command!.ManuelFinish = true; //because it could be somebody else's turn.
        await StartNewTurnAsync();
    }
    public override Task ContinueTurnAsync()
    {
        SaveRoot!.StartMessage = "";
        ReloadMeldFrames();
        return base.ContinueTurnAsync();
    }
    protected override async Task PlayCardAsync(int deck)
    {
        bool hadCard = false;
        if (SingleInfo!.MainHandList.ObjectExist(deck))
        {
            SingleInfo.MainHandList.RemoveObjectByDeck(deck);
            hadCard = true;
        }
        if (hadCard == false)
        {
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            var thisMeld = GetMeldFromCard(thisCard);
            SaveRoot!.MeldList.ForEach(tempMeld =>
            {
                tempMeld.CardList.RemoveAllOnly(items => items == deck);
            });
        }
        await _trickPlay!.PlayCardAsync(deck);
    }
    public override bool IsValidMove(int deck)
    {
        if (_model!.Pile1!.PileEmpty())
        {
            return true; //if there is nothing in the pile, you can play anything no matter what
        }
        return base.IsValidMove(deck);
    }
    public MeldClass GetMeldFromCard(Pinochle2PlayerCardInformation thisCard) => SaveRoot!.MeldList.First(thisMeld => thisMeld.Player == WhoTurn && thisMeld.CardList.Any(items => thisCard.Deck == items));
    public override bool CanEnableTrickAreas => !SaveRoot!.ChooseToMeld;
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentScore = 0;
            thisPlayer.TotalScore = 0;
        });
        return Task.CompletedTask;
    }
    private DeckRegularDict<Pinochle2PlayerCardInformation> GetPlayerMeldList(int player)
    {
        var thisList = SaveRoot!.MeldList.Where(items => items.Player == player).Select(items => items.CardList).ToBasicList();
        DeckRegularDict<Pinochle2PlayerCardInformation> output = new();
        thisList.ForEach(firstTemp =>
        {
            var tempList = firstTemp.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
            tempList.ForEach(thisCard =>
            {
                if (output.ObjectExist(thisCard.Deck) == false)
                {
                    output.Add(thisCard);
                }
            });
        });
        return output;
    }
    private void MeldsBackInHand()
    {
        2.Times(x =>
        {
            var tempPlayer = PlayerList![x];
            tempPlayer.MainHandList.AddRange(GetPlayerMeldList(x));
            if (tempPlayer.MainHandList.Count != 12)
            {
                throw new CustomBasicException("Must have 12 cards in hand at end");
            }
        });
    }
    private void ReloadMeldFrames()
    {
        _model!.YourMelds!.HandList.Clear();
        _model.OpponentMelds!.HandList.Clear();
        if (_model.Pile1!.PileEmpty())
        {
            _model.YourMelds.Visible = false;
            _model.OpponentMelds.Visible = false;
            return;
        }
        _model.YourMelds.Visible = true;
        _model.OpponentMelds.Visible = true;
        int myID = PlayerList!.GetSelf().Id;
        2.Times(x =>
        {
            var thisList = GetPlayerMeldList(x);
            thisList.ForEach(thisCard =>
            {
                thisCard.IsUnknown = false;
                thisCard.Visible = true;
            });
            if (myID == 1 && x == 1 || myID == 2 && x == 2)
            {
                _model.YourMelds.PopulateObjects(thisList);
            }
            else
            {
                _model.OpponentMelds.PopulateObjects(thisList);
            }
        });
    }
    public async Task ExchangeDiscardAsync(int deck)
    {
        var thisCard = _model!.Pile1!.GetCardInfo();
        _model.Pile1.RemoveFromPile();
        await Aggregator.AnimatePickUpDiscardAsync(thisCard);
        var newCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        thisCard.Drew = true;
        SingleInfo!.MainHandList.Add(thisCard);
        MeldClass thisMeld = new();
        thisMeld.ClassAValue = EnumClassA.Dix;
        thisMeld.Player = WhoTurn;
        SaveRoot!.MeldList.Add(thisMeld);
        CalculateScore(WhoTurn);
        if (SingleInfo.MainHandList.ObjectExist(deck))
        {
            SingleInfo.MainHandList.RemoveObjectByDeck(deck);
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            SortCards();
        }
        await AnimatePlayAsync(newCard);
        await ContinueTurnAsync();
    }
    private bool DidWinGame
    {
        get
        {
            if (PlayerList.Count != 2)
            {
                throw new CustomBasicException("Must have 2 players only");
            }
            if (PlayerList.First().TotalScore == PlayerList.Last().TotalScore)
            {
                return false;
            }
            SingleInfo = PlayerList.OrderByDescending(items => items.TotalScore).First();
            return true;
        }
    }
    public override async Task EndRoundAsync()
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore += thisPlayer.CurrentScore);
        if (PlayerList.Any(items => items.TotalScore >= SaveRoot!.GameOverAt))
        {
            if (DidWinGame == false)
            {
                SaveRoot!.GameOverAt += 200;
            }
            else
            {
                await ShowWinAsync();
                return;
            }
        }
        await this.RoundOverNextAsync();
    }
    private static bool HasNewMeld(MeldClass oldMeld, MeldClass newMeld)
    {
        if (oldMeld.ClassAValue != EnumClassA.None && newMeld.ClassAValue != EnumClassA.None)
        {
            return false;
        }
        if (oldMeld.ClassBValue != EnumClassB.None && newMeld.ClassBValue != EnumClassB.None)
        {
            return false;
        }
        if (oldMeld.ClassCValue != EnumClassC.None && newMeld.ClassCValue != EnumClassC.None)
        {
            return false;
        }
        return true;
    }
    private void FirstProcessMeld(BasicList<int> thisList)
    {
        var thisMeld = SaveRoot!.MeldList.Where(items => items.Player == WhoTurn && items.CardList.Any(fins => thisList.Contains(fins))).FirstOrDefault();
        var temps = thisList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
        if (thisMeld == null)
        {
            temps.ForEach(thisCard =>
            {
                thisCard.Drew = true;
                thisCard.IsSelected = false;
            });
            thisMeld = GetMeldFromList(temps);
            thisMeld.Player = WhoTurn;
            thisMeld.CardList = thisList;
            SaveRoot.MeldList.Add(thisMeld);
            return;
        }
        var tempMeld = GetMeldFromList(temps);
        var nextList = thisList.Where(items => thisMeld.CardList.Any(fins => fins == items) == false).ToBasicList();
        var firstTemp = SaveRoot.MeldList.ToBasicList();
        firstTemp.RemoveSpecificItem(thisMeld);
        nextList.RemoveAllOnly(items =>
        {
            return firstTemp.Any(fins =>
            {
                return fins.CardList.Any(z => z == items);
            });
        });
        if (thisMeld.ClassAValue != EnumClassA.None && tempMeld.ClassAValue != EnumClassA.None && tempMeld.ClassAValue > thisMeld.ClassAValue)
        {
            thisMeld.ClassAValue = tempMeld.ClassAValue;
        }
        else if (thisMeld.ClassBValue != EnumClassB.None && tempMeld.ClassBValue != EnumClassB.None && tempMeld.ClassBValue > thisMeld.ClassBValue)
        {
            thisMeld.ClassBValue = tempMeld.ClassBValue;
        }
        else if (thisMeld.ClassCValue != EnumClassC.None && tempMeld.ClassCValue != EnumClassC.None && tempMeld.ClassCValue > thisMeld.ClassCValue)
        {
            thisMeld.ClassCValue = tempMeld.ClassCValue;
        }
        else if (HasNewMeld(thisMeld, tempMeld) == true)
        {
            tempMeld.Player = WhoTurn;
            tempMeld.CardList = thisList;
            SaveRoot.MeldList.Add(tempMeld);
            return;
        }
        thisMeld.CardList.AddRange(nextList);
    }
    public MeldClass GetMeldFromList(IDeckDict<Pinochle2PlayerCardInformation> thisList)
    {
        MeldClass defaultMeld = new();
        defaultMeld.ClassAValue = EnumClassA.None;
        defaultMeld.ClassBValue = EnumClassB.None;
        defaultMeld.ClassCValue = EnumClassC.None;
        if (thisList.Count == 1 && thisList.Single().Value == EnumRegularCardValueList.Nine && thisList.Single().Suit == SaveRoot!.TrumpSuit)
        {
            defaultMeld.ClassAValue = EnumClassA.Dix;
            return defaultMeld;
        }
        if (thisList.Any(items => items.Value == EnumRegularCardValueList.Nine))
        {
            return defaultMeld;
        }
        int suitCount = thisList.GroupBy(items => items.Suit).Count();
        int numberCount = thisList.GroupBy(items => items.Value).Count();
        if (suitCount == 1 && numberCount == 5 && thisList.First().Suit == SaveRoot!.TrumpSuit)
        {
            defaultMeld.ClassAValue = EnumClassA.Flush;
            return defaultMeld;
        }
        if (suitCount == 4 && numberCount == 1 && thisList.Count == 4)
        {
            var card = thisList.First();
            if (card.Value == EnumRegularCardValueList.Jack)
            {
                defaultMeld.ClassBValue = EnumClassB.J;
            }
            else if (card.Value == EnumRegularCardValueList.Queen)
            {
                defaultMeld.ClassBValue = EnumClassB.Q;
            }
            else if (card.Value == EnumRegularCardValueList.King)
            {
                defaultMeld.ClassBValue = EnumClassB.K;
            }
            else if (card.Value == EnumRegularCardValueList.HighAce)
            {
                defaultMeld.ClassBValue = EnumClassB.A;
            }
            else
            {
                defaultMeld.ClassBValue = EnumClassB.None;
            }
            return defaultMeld;
        }
        if (thisList.Count == 2)
        {
            if (thisList.All(items =>
            {
                if (items.Value == EnumRegularCardValueList.Jack && items.Suit == EnumSuitList.Diamonds)
                {
                    return true;
                }
                if (items.Value == EnumRegularCardValueList.Queen && items.Suit == EnumSuitList.Spades)
                {
                    return true;
                }
                return false;
            }))
            {
                defaultMeld.ClassCValue = EnumClassC.Pinochle;
                return defaultMeld;
            }
            if (suitCount == 1 && numberCount == 2)
            {
                if (thisList.All(items =>
                {
                    if (items.Value == EnumRegularCardValueList.Queen || items.Value == EnumRegularCardValueList.King)
                    {
                        return true;
                    }
                    return false;
                }))
                {
                    if (thisList.First().Suit == SaveRoot!.TrumpSuit)
                    {
                        defaultMeld.ClassAValue = EnumClassA.RoyalMarriage;
                    }
                    else
                    {
                        defaultMeld.ClassAValue = EnumClassA.Marriage;
                    }
                    return defaultMeld;
                }
            }
            defaultMeld.ClassAValue = EnumClassA.None;
        }
        return defaultMeld;
    }
    public async Task MeldAsync(BasicList<int> thisList)
    {
        FirstProcessMeld(thisList);
        thisList.ForEach(thisCard =>
        {
            if (SingleInfo!.MainHandList.ObjectExist(thisCard))
            {
                SingleInfo.MainHandList.RemoveObjectByDeck(thisCard);
            }
        });
        CalculateScore(WhoTurn);
        ReloadMeldFrames();
        _command.UpdateAll();
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
        if (thisList.Count == 1)
        {
            _toast.ShowUserErrorToast("Since the dix was played; can make one more meld");
            await ContinueTurnAsync();
            return;
        }
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model!.YourMelds!.EndTurn();
        }
        else
        {
            _model!.OpponentMelds!.EndTurn();
        }
        CalculateScore(WhoTurn);
        await EndTurnAsync();
    }
}