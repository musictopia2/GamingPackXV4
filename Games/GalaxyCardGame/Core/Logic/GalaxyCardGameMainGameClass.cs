namespace GalaxyCardGame.Core.Logic;
[SingletonGame]
public class GalaxyCardGameMainGameClass
    : TrickGameClass<EnumSuitList, GalaxyCardGameCardInformation, GalaxyCardGamePlayerItem, GalaxyCardGameSaveInfo>
        , IMiscDataNM, ISerializable
{
    private readonly GalaxyCardGameVMData _model;
    private readonly CommandContainer _command;
    private readonly GalaxyCardGameGameContainer _gameContainer;
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IToast _toast;
    private readonly RummyProcesses<EnumSuitList, EnumRegularColorList, GalaxyCardGameCardInformation> _rummys;
    public GalaxyCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        GalaxyCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<GalaxyCardGameCardInformation> cardInfo,
        CommandContainer command,
        GalaxyCardGameGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        GalaxyDelegates delegates,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _aTrick = aTrick;
        _toast = toast;
        delegates.PlayerGetCards = (() => PlayerGetCards);
        _rummys = new RummyProcesses<EnumSuitList, EnumRegularColorList, GalaxyCardGameCardInformation>();
    }
    private bool PlayerGetCards { get; set; } = true; //has to be proven false.
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        PlayerList!.ForEach(thisPlayer => thisPlayer.LoadPiles(this, _model, _command, _toast));
        await PlayerList.ForEachAsync(async thisPlayer =>
        {
            DeckRegularDict<GalaxyCardGameCardInformation> thisList = await js1.DeserializeObjectAsync<DeckRegularDict<GalaxyCardGameCardInformation>>(thisPlayer.PlanetData);
            thisPlayer.PlanetHand!.HandList.ReplaceRange(thisList);
            thisPlayer.MainHandList.ForEach(thisCard =>
            {
                thisCard.Visible = true;
                thisCard.IsUnknown = false;
            });
            int x = thisPlayer.SavedMoonList.Count;
            x.Times(Items =>
            {
                MoonClass thisSet = new(_command);
                thisPlayer.Moons!.CreateNewSet(thisSet);
            });
            thisPlayer.Moons!.LoadSets(thisPlayer.SavedMoonList);
        });
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        _rummys.HasSecond = false;
        _rummys.HasWild = false;
        IsLoaded = true;
    }
    public override async Task PopulateSaveRootAsync()
    {
        await PlayerList!.ForEachAsync(async thisPlayer =>
        {
            thisPlayer.PlanetData = await js1.SerializeObjectAsync(thisPlayer.PlanetHand!.HandList.ToRegularDeckDict());
            thisPlayer.SavedMoonList = thisPlayer.Moons!.SavedSets();
        });
        await base.PopulateSaveRootAsync();
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        if (SaveRoot!.GameStatus == EnumGameStatus.PlaceSets)
        {
            await EndTurnAsync();
            return;
        }
        var moveList = SingleInfo!.MainHandList.Where(thisCard => IsValidMove(thisCard.Deck)).ToRegularDeckDict();
        await PlayCardAsync(moveList.GetRandomItem().Deck);
    }
    protected override Task AddCardAsync(GalaxyCardGameCardInformation thisCard, GalaxyCardGamePlayerItem tempPlayer)
    {
        thisCard.Visible = true;
        thisCard.IsUnknown = false;
        return base.AddCardAsync(thisCard, tempPlayer);
    }
    private bool IsBeginning()
    {
        if (SaveRoot!.LastWin > 0)
        {
            return WhoTurn == SaveRoot.LastWin;
        }
        return WhoTurn == WhoStarts;
    }
    protected override async Task ShowHumanCanPlayAsync()
    {
        await base.ShowHumanCanPlayAsync();
        SingleInfo!.ReportChange();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        LoadVM();
        _aTrick!.ClearBoard();
        if (PlayerList.First().Moons == null)
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.LoadPiles(this, _model, _command, _toast);
            });
        }
        SaveRoot.GameStatus = EnumGameStatus.PlaceSets;
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.Moons!.ClearBoard();
            thisPlayer.PlanetHand!.ClearHand();
            thisPlayer.TricksWon = 0;
        });
        return base.StartSetUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        BasicList<int> thisList;
        DeckRegularDict<GalaxyCardGameCardInformation> thisCol;
        switch (status)
        {
            case "newmoon":
                thisList = await js1.DeserializeObjectAsync<BasicList<int>>(content);
                thisCol = thisList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
                await PlayNewMoonAsync(thisCol);
                return;
            case "createplanet":
                thisList = await js1.DeserializeObjectAsync<BasicList<int>>(content);
                thisCol = thisList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
                await CreatePlanetAsync(thisCol);
                return;
            case "expandmoon":
                SendExpandedMoon temps = await js1.DeserializeObjectAsync<SendExpandedMoon>(content);
                await AddToMoonAsync(temps.Deck, temps.MoonID);
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
        if (IsBeginning())
        {
            if (SaveRoot!.LastWin > 0)
            {
                WhoTurn = SaveRoot.LastWin;
            }
            SingleInfo = PlayerList.GetWhoPlayer();
            this.ShowTurn();
            SaveRoot.GameStatus = EnumGameStatus.WinTrick;
            PlayerGetCards = false;
            await DrawAsync();
            return;
        }
        await StartNewTurnAsync();
    }
    private int WhoWonTrick(DeckRegularDict<GalaxyCardGameCardInformation> thisCol)
    {
        var leadCard = thisCol.First();
        var tempList = thisCol.ToRegularDeckDict();
        tempList.RemoveSpecificItem(leadCard);
        if (!tempList.Any(items => items.Suit == leadCard.Suit && items.Value > leadCard.Value))
        {
            return leadCard.Player;
        }
        return WhoTurn;
    }
    public override bool IsValidMove(int deck)
    {
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (thisCard.Value == EnumRegularCardValueList.HighAce)
        {
            return false; //ace is high
        }
        if (thisCard.Value == EnumRegularCardValueList.Five && thisCard.Suit == EnumSuitList.Clubs)
        {
            return false; //because the 5 of clubs is automatic planet.
        }
        var thisList = SaveRoot!.TrickList;
        if (thisList.Count == 0)
        {
            return true;
        }
        var originalCard = thisList.First();
        if (originalCard.Suit == thisCard.Suit)
        {
            return true;
        }
        var tempList = SingleInfo!.MainHandList.Where(items => items.Value != EnumRegularCardValueList.HighAce).ToRegularDeckDict();
        tempList.RemoveAllAndObtain(items => items.Value == EnumRegularCardValueList.Five && items.Suit == EnumSuitList.Clubs);
        return (!tempList.Any(items => items.Suit == originalCard.Suit));
    }
    public override async Task EndTrickAsync()
    {
        var trickList = SaveRoot!.TrickList;
        int wins = WhoWonTrick(trickList);
        GalaxyCardGamePlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        await _aTrick!.AnimateWinAsync(wins);
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(SaveRoot.WinningCard.Deck);
        WhoTurn = wins;
        SaveRoot.LastWin = WhoTurn;
        SingleInfo = PlayerList.GetWhoPlayer();
        thisCard.IsUnknown = false;
        thisCard.Drew = false;
        SingleInfo.MainHandList.Add(thisCard);
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            SortCards();
        }
        SaveRoot.TrickList.ForEach(nextCard => _model!.Pile1!.AddCard(nextCard));
        _aTrick.ClearBoard();
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        SaveRoot.WinningCard = new GalaxyCardGameCardInformation();
        PlayerGetCards = true;
        await DrawAsync();
    }
    protected override async Task AfterDrawingAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        SingleInfo = PlayerList.GetWhoPlayer();
        this.ShowTurn();
        SaveRoot!.GameStatus = EnumGameStatus.PlaceSets;
        await ContinueTurnAsync();
    }
    protected override async Task PlayerReceivesNoCardsAfterDrawingAsync(GalaxyCardGameCardInformation thisCard)
    {
        SaveRoot!.WinningCard = thisCard;
        await StartNewTrickAsync();
    }
    private async Task StartNewTrickAsync()
    {
        _aTrick!.ClearBoard();
        _command!.ManuelFinish = true; //because it could be somebody else's turn.
        await StartNewTurnAsync();
    }

    public bool CanEndTurn()
    {
        if (SaveRoot!.GameStatus != EnumGameStatus.PlaceSets)
        {
            return false;
        }
        if (SingleInfo!.MainHandList.Any(items => items.Value == EnumRegularCardValueList.Five && items.Suit == EnumSuitList.Clubs))
        {
            return false;
        }
        if (SingleInfo.PlanetHand!.HandList.Count == 0)
        {
            return true;
        }
        return !SingleInfo.MainHandList.Any(items => items.Value == EnumRegularCardValueList.HighAce);
    }
    public override bool CanEnableTrickAreas
    {
        get
        {
            return SaveRoot!.GameStatus == EnumGameStatus.WinTrick;
        }
    }
    public bool HasAutomaticPlanet()
    {
        return SingleInfo!.MainHandList.Any(items => items.Value == EnumRegularCardValueList.Five && items.Suit == EnumSuitList.Clubs);
    }
    private async Task FinishedEndAsync()
    {
        if (SingleInfo!.MainHandList.Count == 0)
        {
            await ShowWinAsync();
            return;
        }
        await ContinueTurnAsync();
    }
    public async Task AddToMoonAsync(int deck, int setNumber)
    {
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        var thisMoon = SingleInfo!.Moons!.GetIndividualSet(setNumber);
        thisMoon.AddCard(thisCard);
        SingleInfo.MainHandList.RemoveObjectByDeck(deck);
        await FinishedEndAsync();
    }
    public bool CanAddToMoon(MoonClass thisMoon, out GalaxyCardGameCardInformation? whatCard, out string message)
    {
        whatCard = null;
        message = "";
        if (_model!.PlayerHand1!.HowManySelectedObjects == 0)
        {
            message = "There are no cards selected";
            return false;
        }
        int deck = _model.PlayerHand1.ObjectSelected();
        whatCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        if (thisMoon.CanExpand(whatCard) == false)
        {
            whatCard = null;
            message = "Sorry, this card cannot be used to expand the set";
            return false;
        }
        return true;
    }
    public bool HasValidMoon(DeckRegularDict<GalaxyCardGameCardInformation> thisList)
    {
        if (thisList.Count == 0 || HasAutomaticPlanet())
        {
            return false;
        }
        if (thisList.All(items => items.Value == EnumRegularCardValueList.HighAce))
        {
            return true; //aces are a guarantee.
        }
        if (thisList.Any(items => items.Value == EnumRegularCardValueList.HighAce))
        {
            return false; //ace has to be by itself.
        }
        if (thisList.Count == 1)
        {
            return false; //if no ace, then needs at least 2.
        }
        if (thisList.Count >= 3)
        {
            if (_rummys!.IsNewRummy(thisList, thisList.Count, EnumRummyType.Runs))
            {
                return true;
            }
        }
        return _rummys!.IsNewRummy(thisList, thisList.Count, EnumRummyType.Sets);
    }
    public bool HasValidPlanet(DeckRegularDict<GalaxyCardGameCardInformation> thisList)
    {
        if (thisList.Count == 0)
        {
            return false;
        }
        if (thisList.Count > 1)
        {
            if (HasAutomaticPlanet())
            {
                return false;
            }
            if (thisList.HasDuplicates(items => items.Suit) == false)
            {
                return false;
            }
            int totalPoints = thisList.Sum(items => items.Points);
            return totalPoints >= 15 && totalPoints <= 18;
        }
        return HasAutomaticPlanet();
    }
    public async Task PlayNewMoonAsync(IDeckDict<GalaxyCardGameCardInformation> thisList)
    {
        thisList.ForEach(thisCard =>
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(thisCard.Deck);
        });
        int counts = thisList.GroupBy(items => items.Value).Count();
        EnumWhatSets whatSet;
        if (counts == 1)
        {
            whatSet = EnumWhatSets.Kinds;
        }
        else
        {
            whatSet = EnumWhatSets.Runs;
            thisList = thisList.OrderBy(items => items.Value).ToRegularDeckDict();
        }
        MoonClass thisMoon = new(_command);
        thisMoon.CreateNewMoon(thisList, whatSet);
        SingleInfo!.Moons!.CreateNewSet(thisMoon);
        await FinishedEndAsync();
    }
    public async Task CreatePlanetAsync(IDeckDict<GalaxyCardGameCardInformation> thisList)
    {
        if (thisList.Count == 1)
        {
            var thisCard = thisList.Single();
            thisCard.IsSelected = false;
            thisCard.Drew = false;
            if (thisCard.Suit != EnumSuitList.Clubs)
            {
                throw new CustomBasicException("Only clubs can be a planet with only one card");
            }
            if (thisCard.Value != EnumRegularCardValueList.Five)
            {
                throw new CustomBasicException("Only the 5 of clubs can be used for a planet with only one card");
            }
            var newList = SingleInfo!.PlanetHand!.HandList.ToRegularDeckDict();
            SingleInfo.PlanetHand.PopulateObjects(thisList);
            SingleInfo.MainHandList.AddRange(newList);
            SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                SortCards();
            }
            await ContinueTurnAsync();
            return;
        }
        SingleInfo!.PlanetHand!.PopulateObjects(thisList);
        thisList.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            thisCard.Drew = false;
        });
        SingleInfo.MainHandList.RemoveSelectedItems(thisList);
        await ContinueTurnAsync();
    }
}