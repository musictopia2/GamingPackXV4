namespace MonopolyCardGame.Core.Logic;
[SingletonGame]
public class MonopolyCardGameMainGameClass
    : CardGameClass<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem, MonopolyCardGameSaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly MonopolyCardGameVMData _model;
    private readonly CommandContainer _command;
    private readonly MonopolyCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    public MonopolyCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        MonopolyCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<MonopolyCardGameCardInformation> cardInfo,
        CommandContainer command,
        MonopolyCardGameGameContainer gameContainer,
        ISystemError error,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
        _privateAutoResume = privateAutoResume;
        _gameContainer.ProcessTrade = ProcessTrade;
    }
    private bool _doContinue;
    public override async Task FinishGetSavedAsync()
    {
        LoadTradePiles();
        _doContinue = true;
        _model.TempSets1.ClearBoard();
        await PlayerList!.ForEachAsync(async thisPlayer =>
        {
            var thisList = await js1.DeserializeObjectAsync<DeckRegularDict<MonopolyCardGameCardInformation>>(thisPlayer.TradeString);
            thisPlayer.TradePile!.HandList = new DeckRegularDict<MonopolyCardGameCardInformation>(thisList);
        });
        PlayerList!.ForEach(player =>
        {
            if (player.AdditionalCards.Count > 0)
            {
                player.MainHandList.AddRange(player.AdditionalCards);
                player.AdditionalCards.Clear();
            }
        });
        if (SaveRoot.GameStatus == EnumWhatStatus.Other && SaveRoot.ManuelStatus == EnumManuelStatus.Final)
        {
            StartProcessAfterDrawing5Cards();
        }
        var player = PlayerList.GetSelf();
        _model.TempHand1.HandList = player.MainHandList; //trying to hook up.
        bool rets;
        rets = await _privateAutoResume.HasAutoResumeAsync(_gameContainer, _model);
        if (rets)
        {
            await _privateAutoResume.RestoreStateAsync(_gameContainer, _model);
        }
        await base.FinishGetSavedAsync();
    }
    protected override async Task ComputerTurnAsync()
    {
        await Task.CompletedTask;
    }
    public override async Task PopulateSaveRootAsync()
    {
        await PlayerList!.ForEachAsync(async thisPlayer =>
        {
            thisPlayer.TradeString = await js1.SerializeObjectAsync(thisPlayer.TradePile!.HandList);
        });
        MonopolyCardGamePlayerItem Self = PlayerList!.GetSelf();
        Self.AdditionalCards = _model.TempSets1!.ListAllObjects(); //try this way (?)
        await base.PopulateSaveRootAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (isBeginning)
        {
            LoadTradePiles();
        }
        else
        {
            _model.TempSets1.ClearBoard(); //i think.
        }
        SaveRoot!.ImmediatelyStartTurn = true;
        SaveRoot.GameStatus = EnumWhatStatus.DrawOrTrade;
        _doContinue = true;
        _model!.AdditionalInfo1!.Clear();
        return base.StartSetUpAsync(isBeginning);
    }
    protected override async Task LastPartOfSetUpBeforeBindingsAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TradePile!.ClearBoard(_model!.Deck1!.DrawCard());
        });
        var player = PlayerList.GetSelf();
        _model.TempHand1.HandList = player.MainHandList;
        await base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "goout":
                SingleInfo = PlayerList!.GetWhoPlayer();
                await ProcessGoingOutAsync();
                Network!.IsEnabled = true;
                return;
            case "trade1":
                var thisCol = await content.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
                thisCol.ForEach(thisCard => SingleInfo.TradePile!.AddCard(thisCard.Deck));
                SingleInfo.TradePile!.SetPreviousStatus();
                _command.UpdateAll();
                Network!.IsEnabled = true;
                return;
            case "trade2":
                SendTrade thisSend = await js1.DeserializeObjectAsync<SendTrade>(content);
                var tempPlayer = PlayerList![thisSend.Player];
                TradePile newTrade = tempPlayer.TradePile!;
                var tempCollection = thisSend.CardList.GetNewObjectListFromDeckList(_gameContainer.DeckList!).ToRegularDeckDict();
                ProcessTrade(newTrade, tempCollection, SingleInfo!.TradePile!);
                //the client does not need to know about ending turn.
                await ContinueTurnAsync();
                return;
            case "finishedsets":
                SingleInfo = PlayerList.GetWhoPlayer();
                var list2 = await GetSetInfoAsync(content);
                await FinishManualProcessingAsync(list2);
                await ContinueTurnAsync(); //try this
                return;
            case "putback":
                SingleInfo = PlayerList.GetWhoPlayer();
                await SingleInfo.TradePile!.PutBackAsync();
                _command.UpdateAll();
                Network!.IsEnabled = true;
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalMoney = 0;
            thisPlayer.PreviousMoney = 0;
        });
        return Task.CompletedTask;
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        if (SaveRoot!.GameStatus != EnumWhatStatus.LookOnly)
        {
            if (SingleInfo!.ObjectCount > 10)
            {
                SaveRoot.GameStatus = EnumWhatStatus.Either;
            }
            else
            {
                SaveRoot.GameStatus = EnumWhatStatus.DrawOrTrade;
            }
        }
        else
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
            {
                await ProcessEndAsync(); //the computer will calculate the score this way.
                await EndTurnAsync();
                return;
            }
            if (SingleInfo.HasChance(_model))
            {
                await ContinueTurnAsync(); //hopefully simple enough because they got 0 points.
                return;
            }
            SaveRoot.GameStatus = EnumWhatStatus.Other;
            SaveRoot.ManuelStatus = EnumManuelStatus.OthersLayingDown;
        }
        await ContinueTurnAsync();
    }
    public override async Task EndRoundAsync()
    {
        if (PlayerList.Any(items => items.TotalMoney >= 10000))
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.TotalMoney).First();
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    public async Task ForceAllowPlayAsync()
    {
        await ShowHumanCanPlayAsync();
        _command.UpdateAll();
    }
    public override async Task EndTurnAsync()
    {

        SingleInfo = PlayerList!.GetWhoPlayer();
        if (SingleInfo.CanSendMessage(BasicData!))
        {
            await Network!.SendEndTurnAsync();
        }
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self)
        {
            SingleInfo.MainHandList.UnhighlightObjects();
        }
        else if (_gameContainer.AlreadyDrew)
        {
            _model.PlayerHand1!.EndTurn();
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.AdditionalInfo1!.Clear();
        }
        PlayerList.ForEach(thisPlayer => thisPlayer.TradePile!.EndTurn());
        if (SaveRoot!.GameStatus == EnumWhatStatus.LookOnly)
        {
            var previousOne = SaveRoot.WhoWentOut - 1;
            if (previousOne == 0)
            {
                previousOne = PlayerList.Count;
            }
            if (previousOne == WhoTurn)
            {
                await EndRoundAsync();
                return;
            }
        }
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    #region "Advanced Processes"
    private async Task DrawUpTo5Async()
    {
        LeftToDraw = 5;
        _doContinue = false;
        await DrawAsync();
    }
    public async Task ProcessGoingOutAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SaveRoot!.WhoWentOut = WhoTurn;
        _model!.Status = SingleInfo.NickName + " went out.  Finishing the round";
        PlayerList.ForEach(thisPlayer =>
        {
            thisPlayer.TradePile!.RemoveCards();
        });
        await DrawUpTo5Async();
    }
    protected override bool ShowNewCardDrawn(MonopolyCardGamePlayerItem TempPlayer)
    {
        if (_doContinue == false)
        {
            return true;
        }
        return base.ShowNewCardDrawn(TempPlayer);
    }
    protected override async Task AfterDrawingAsync()
    {
        if (_doContinue)
        {
            SaveRoot!.GameStatus = EnumWhatStatus.Discard;
            await base.AfterDrawingAsync();
            return;
        }
        SaveRoot.GameStatus = EnumWhatStatus.Other;
        SaveRoot.ManuelStatus = EnumManuelStatus.Final;
        StartProcessAfterDrawing5Cards();
    }
    private void StartProcessAfterDrawing5Cards()
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.Status = "Needs to manually figure out the monopolies for going out.";
        }
        else
        {
            _model.Status = $"Waiting for {SingleInfo.NickName} to manually figure out the monopolies for going out.";
        }
    }
    public void ProcessTrade(TradePile newTrade, DeckRegularDict<MonopolyCardGameCardInformation> oldCollection, TradePile yourTrade)
    {
        newTrade.GetNumberOfItems(oldCollection.Count);
        SingleInfo = PlayerList![newTrade.GetPlayerIndex];
        yourTrade.GetNumberOfItems(oldCollection.Count);
        SingleInfo = PlayerList.GetWhoPlayer();
        SaveRoot!.GameStatus = EnumWhatStatus.Discard;
    }
    internal void CreateTradePile(MonopolyCardGamePlayerItem tempPlayer)
    {
        tempPlayer.TradePile = new TradePile(_gameContainer, _model, tempPlayer.Id, _toast);
    }
    private void LoadTradePiles()
    {
        var tempList = PlayerList!.GetAllPlayersStartingWithSelf();
        tempList.ForEach(CreateTradePile);
    }
    private async Task ProcessEndAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        //rethink later.
        var newScore = CalculateScore(WhoTurn, false, out DeckRegularDict<MonopolyCardGameCardInformation> newGroup);
        await FinalProcessAsync(newGroup, newScore);
    }
    private async Task FinalProcessAsync(DeckRegularDict<MonopolyCardGameCardInformation> newGroup, decimal newScore)
    {
        var thisTrade = SingleInfo!.TradePile;
        thisTrade!.RemoveCards();
        newGroup.ForEach(thisCard =>
        {
            if (SingleInfo.MainHandList.ObjectExist(thisCard.Deck))
            {
                SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);

            }
            thisTrade.AddCard(thisCard.Deck); //still needs this because may have already been removed now.
        });
        SingleInfo.PreviousMoney = newScore;
        SingleInfo.TotalMoney += newScore;
        _command.UpdateAll(); //just in case its needed for blazor (?)
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
    }
    internal static DeckRegularDict<MonopolyCardGameCardInformation> HouseCollection(DeckRegularDict<MonopolyCardGameCardInformation> whatCol)
    {
        DeckRegularDict<MonopolyCardGameCardInformation> output = [];
        if (whatCol.Any(items => items.WhatCard == EnumCardType.IsChance || items.WhatCard == EnumCardType.IsHouse) == false)
        {
            return output; //because there are no houses or chance which is wild.
        }
        MonopolyCardGameCardInformation thisCard;
        for (int x = 1; x <= 4; x++)
        {
            if (whatCol.Any(items => items.HouseNum == x) == false)
            {
                if (!whatCol.Any(items => items.WhatCard == EnumCardType.IsChance))
                {
                    return output;
                }
                thisCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
            }
            else
            {
                thisCard = whatCol.First(items => items.HouseNum == x);
            }
            whatCol.RemoveSpecificItem(thisCard);
            output.Add(thisCard);
        }
        if (whatCol.Any(items => items.WhatCard == EnumCardType.IsHotel) == true)
        {
            thisCard = whatCol.First(items => items.WhatCard == EnumCardType.IsHotel);
        }
        else if (!whatCol.Any(items => items.WhatCard == EnumCardType.IsChance))
        {
            return output;
        }
        else
        {
            thisCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
        }
        whatCol.RemoveSpecificItem(thisCard);
        output.Add(thisCard);
        return output;
    }
    internal static DeckRegularDict<MonopolyCardGameCardInformation> MonopolyCol(DeckRegularDict<MonopolyCardGameCardInformation> whatCol, int whatGroup, EnumCardType whatType)
    {
        DeckRegularDict<MonopolyCardGameCardInformation> output = [];
        int numWilds = whatCol.Count(items => items.WhatCard == EnumCardType.IsChance);
        int howMany = whatCol.Count(items => items.WhatCard == whatType);
        if (howMany == 0 && whatGroup == 0)
        {
            return output;
        }
        var temps = whatCol.Where(items => items.WhatCard == whatType).ToRegularDeckDict();
        if (whatType == EnumCardType.IsUtilities)
        {
            if (howMany > 2)
            {
                throw new CustomBasicException("Can't have more than 2 utilities");
            }
            if (howMany == 2 || howMany == 1 && numWilds > 0)
            {
                output.AddRange(temps);
                whatCol.RemoveGivenList(temps);
                if (howMany == 2)
                {
                    return output;
                }
            }
            if (numWilds == 0)
            {
                return output;
            }
            var utilCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
            whatCol.RemoveSpecificItem(utilCard);
            output.Add(utilCard);
            return output;
        }
        if (whatType == EnumCardType.IsRailRoad)
        {
            if (howMany > 4)
            {
                throw new CustomBasicException("Can't have more than 4 railroads");
            }
            if (howMany > 1 || howMany == 1 && numWilds > 0)
            {
                if (numWilds == 0 && howMany == 1)
                {
                    return output;
                }
                output.AddRange(temps);
                whatCol.RemoveGivenList(temps);
            }
            if (numWilds > 0) //can only use one wild for rail roads
            {
                var tempCard = whatCol.First(items => items.WhatCard == EnumCardType.IsChance);
                whatCol.RemoveSpecificItem(tempCard);
                output.Add(tempCard);
            }
            return output;
        }
        temps = whatCol.Where(Items => Items.Group == whatGroup).ToRegularDeckDict();
        howMany = temps.Count;
        if (howMany == 0)
        {
            return output; //i think we need at least one to even use a chance.
        }
        int numberNeeded;
        if (whatGroup == 1 || whatGroup == 8)
        {
            numberNeeded = 2;
        }
        else
        {
            numberNeeded = 3;
        }
        if (howMany > numberNeeded)
        {
            throw new CustomBasicException("Can't have more than the needed number");
        }
        if (howMany == numberNeeded)
        {
            output.AddRange(temps);
            whatCol.RemoveGivenList(temps);
            return output;
        }
        if (numWilds + howMany < numberNeeded)
        {
            return output;
        }
        int wildsNeeded = numberNeeded - howMany;
        output.AddRange(temps);
        whatCol.RemoveGivenList(temps);
        temps = whatCol.Where(Items => Items.WhatCard == EnumCardType.IsChance).Take(wildsNeeded).ToRegularDeckDict();
        output.AddRange(temps);
        whatCol.RemoveGivenList(temps);
        return output;
    }
    private static decimal CalculateMoneyFromGroup(ListInfo thisList, int numTokens) //looks like no ref needed because value does not change anyways
    {
        int output;
        if (thisList.WhatCard == EnumCardType.IsRailRoad)
        {
            if (thisList.RailRoads == 2)
            {
                output = 250;
            }
            else if (thisList.RailRoads == 3)
            {
                output = 500;
            }
            else
            {
                output = 1000;
            }
        }
        else if (thisList.WhatCard == EnumCardType.IsUtilities)
        {
            output = 500;
        }
        else
        {
            int bases;
            if (thisList.Group == 8)
            {
                bases = 400;
            }
            else if (thisList.Group == 7)
            {
                bases = 350;
            }
            else if (thisList.Group == 6)
            {
                bases = 300;
            }
            else if (thisList.Group == 5)
            {
                bases = 250;
            }
            else if (thisList.Group == 4)
            {
                bases = 200;
            }
            else if (thisList.Group == 3)
            {
                bases = 150;
            }
            else if (thisList.Group == 2)
            {
                bases = 100;
            }
            else if (thisList.Group == 1)
            {
                bases = 50;
            }
            else
            {
                throw new CustomBasicException("Must be between 1 and 8");
            }
            if (thisList.NumberOfHouses > 0)
            {
                int extras = thisList.NumberOfHouses * bases;
                bases += extras; //their suggestion of one liner is too confusing.
                if (thisList.HasHotel)
                {
                    bases += 500;
                }
            }
            output = bases;
        }
        if (numTokens == 0)
        {
            return output;
        }
        int tt = numTokens + 1;
        output *= tt;
        return output;
    }
    private int MostMrs()
    {
        int mostss = 0;
        int output = 0;
        PlayerList!.ForEach(thisPlayer =>
        {
            var tempList = thisPlayer.MainHandList.ToRegularDeckDict();
            var nexts = thisPlayer.TradePile!.HandList.ToRegularDeckDict();
            tempList.AddRange(nexts);
            int manys = tempList.Count(items => items.WhatCard == EnumCardType.IsMr);
            if (manys == mostss)
            {
                output = 0;
            }
            else if (manys > mostss)
            {
                output = thisPlayer.Id;
                mostss = manys;
            }
        });
        return output;
    }
    private decimal CalculateScore(int player, bool wentOut, out DeckRegularDict<MonopolyCardGameCardInformation> newGroup)
    {
        newGroup = [];
        SingleInfo = PlayerList![player];
        var tempCol = SingleInfo.MainHandList.ToRegularDeckDict();
        if (wentOut == false && tempCol.Any(items => items.WhatCard == EnumCardType.IsChance))
        {
            return 0;//if you did not go out, you get 0 for having a chance in your hand.
        }
        int hadMostMrs = MostMrs(); //i think even if a player did not go out, they could get the points for the most mr monopolies.
        decimal tempScore = 0;
        if (hadMostMrs == player)
        {
            tempScore += 1000;
            newGroup.AddRange(tempCol.Where(items => items.WhatCard == EnumCardType.IsMr));
        }
        DeckRegularDict<MonopolyCardGameCardInformation> thisCol = [];
        thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsGo).ToRegularDeckDict();
        tempScore += thisCol.Count * 200;
        newGroup.AddRange(thisCol);
        return tempScore;
    }
    #endregion
    #region "ManuelSetProcesses"
    public bool HasAllValidMonopolies()
    {
        for (int x = 1; x <= _model.TempSets1.HowManySets; x++)
        {
            var list = _model.WhatSet(x);
            if (list.Count > 0)
            {
                if (list.CanGoOut(true) == false)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public BasicList<TempInfo> ListValidSets()
    {
        BasicList<TempInfo> output = [];
        for (int x = 1; x <= _model.TempSets1.HowManySets; x++)
        {
            var list = _model.WhatSet(x);
            if (list.CanGoOut(true))
            {
                output.Add(new TempInfo()
                {
                    CardList = list.ToRegularDeckDict(),
                    SetNumber = x
                });
            }
        }
        return output;
    }
    private static decimal CalculateScore(IDeckDict<MonopolyCardGameCardInformation> list)
    {
        var tokenList = list.Where(x => x.WhatCard == EnumCardType.IsToken).ToRegularDeckDict();
        int tokens = tokenList.Count;
        BasicList<ListInfo> listMons = [];
        var possList = list.Where(items => items.Group > 0).GroupOrderDescending(items => items.Group).ToBasicList();
        int rawRailRoads = list.Count(x => x.WhatCard == EnumCardType.IsRailRoad);
        bool hasUtilities = list.Any(x => x.WhatCard == EnumCardType.IsUtilities);
        int chances = list.Count(x => x.WhatCard == EnumCardType.IsChance);
        int totalChances = chances;
        ListInfo item = new();
        if (rawRailRoads > 0)
        {
            item.WhatCard = EnumCardType.IsRailRoad;

            if (rawRailRoads + chances <= 4)
            {
                if (rawRailRoads == 3 && chances == 2)
                {
                    rawRailRoads = 4;
                    chances = 1;
                }
                else
                {
                    rawRailRoads += chances;
                    chances = 0;
                }
                rawRailRoads += chances;
            }
            tokens += totalChances;
            item.RailRoads = rawRailRoads;
        }
        else if (hasUtilities)
        {
            item.WhatCard = EnumCardType.IsUtilities;
            if (list.Count(x => x.WhatCard == EnumCardType.IsUtilities) == 1)
            {
                chances--;
            }
        }
        else
        {
            item.WhatCard = EnumCardType.IsProperty;
            var select = list.Where(x => x.Group > 0).First();
            item.Group = select.Group;
            //this means you have what is necessary.
            var count = list.Count(x => x.Group == select.Group);
            int required = RequiredFromGroup(select.Group);
            if (count + chances == required)
            {
                chances = 0;
            }
            else if (count + chances - 1 == required && chances == 2)
            {
                chances = 1;
            }
            //next step is using for missing house.
            int houses = list.Count(x => x.WhatCard == EnumCardType.IsHouse);
            bool hasHotel = list.Any(x => x.WhatCard == EnumCardType.IsHotel);
            if (houses == 4 && hasHotel == false && chances >= 1)
            {
                chances--;
                hasHotel = true;
            }
            else if (houses == 3 && chances == 2)
            {
                houses = 4;
                hasHotel = true;
                chances = 0;
            }
            else if (chances > 0)
            {
                houses += chances;
                chances = 0;
            }
            item.NumberOfHouses = houses;
            item.HasHotel = hasHotel;
        }
        tokens += chances;
        decimal output = CalculateMoneyFromGroup(item, tokens);
        return output;
    }
    private static int RequiredFromGroup(int group)
    {
        if (group == 1 || group == 8)
        {
            return 2;
        }
        return 3;
    }
    private async Task<BasicList<DeckRegularDict<MonopolyCardGameCardInformation>>> GetSetInfoAsync(string message)
    {
        var firstTemp = await js1.DeserializeObjectAsync<BasicList<string>>(message);
        BasicList<DeckRegularDict<MonopolyCardGameCardInformation>> output = [];
        foreach (var thisFirst in firstTemp)
        {
            var thisCol = await thisFirst.GetObjectsFromDataAsync(SingleInfo!.MainHandList);
            output.Add(thisCol);
        }
        return output;
    }
    public static BasicList<DeckRegularDict<MonopolyCardGameCardInformation>> GetSetInfo(BasicList<TempInfo> payLoad)
    {
        BasicList<DeckRegularDict<MonopolyCardGameCardInformation>> output = [];
        foreach (var item in payLoad)
        {
            output.Add(item.CardList);
        }
        return output;
    }
    public async Task FinishManualProcessingAsync(BasicList<DeckRegularDict<MonopolyCardGameCardInformation>> payLoad)
    {
        SaveRoot.GameStatus = EnumWhatStatus.LookOnly; //i think.
        this.StartingStatus();
        decimal score = 0;
        SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
        DeckRegularDict<MonopolyCardGameCardInformation> mans = [];
        foreach (var item in payLoad)
        {
            score += CalculateScore(item);
            mans.AddRange(item);
            foreach (var card in item)
            {
                if (SingleInfo.MainHandList.ObjectExist(card.Deck))
                {
                    SingleInfo.MainHandList.RemoveObjectByDeck(card.Deck);
                }
            }
        }
        bool wentOut;
        if (SaveRoot.ManuelStatus == EnumManuelStatus.Final)
        {
            wentOut = true;
        }
        else
        {
            wentOut = false;
        }
        SaveRoot.ManuelStatus = EnumManuelStatus.None; //not anymore now.
        decimal finalScore = CalculateScore(WhoTurn, wentOut, out DeckRegularDict<MonopolyCardGameCardInformation> lasts);
        decimal totalScore = score + finalScore;
        mans.AddRange(lasts);
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.TempSets1.ClearBoard(); //i think
        }
        await FinalProcessAsync(mans, totalScore);
    }
    #endregion
}