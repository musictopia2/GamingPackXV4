using System.Runtime.CompilerServices;

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
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
        _gameContainer.ProcessTrade = ProcessTrade;
    }
    private bool _doContinue;
    public override async Task FinishGetSavedAsync()
    {
        LoadTradePiles();
        _doContinue = true;
        await PlayerList!.ForEachAsync(async thisPlayer =>
        {
            var thisList = await js1.DeserializeObjectAsync<DeckRegularDict<MonopolyCardGameCardInformation>>(thisPlayer.TradeString);
            thisPlayer.TradePile!.HandList = new DeckRegularDict<MonopolyCardGameCardInformation>(thisList);
        });

        if (SaveRoot.GameStatus == EnumWhatStatus.Other && SaveRoot.ManuelStatus == EnumManuelStatus.WentOutAfterDrawing5Cards)
        {
            StartProcessAfterDrawing5Cards();
        }

        //lots of rethinking is required here now.



        //if (SaveRoot.GameStatus == EnumWhatStatus.ManuallyFigureOutMonopolies)
        //{
        //    StartFiguringOutChance();
        //}
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
        await base.PopulateSaveRootAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (isBeginning)
        {
            LoadTradePiles();
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
                _command.UpdateAll();
                Network!.IsEnabled = true;
                return;
            case "trade2":
                SendTrade thisSend = await js1.DeserializeObjectAsync<SendTrade>(content);
                var tempPlayer = PlayerList![thisSend.Player];
                TradePile newTrade = tempPlayer.TradePile!;
                var tempCollection = thisSend.CardList.GetNewObjectListFromDeckList(_gameContainer.DeckList!).ToRegularDeckDict();
                ProcessTrade(newTrade, tempCollection, SingleInfo!.TradePile!);
                await ContinueTurnAsync();
                return;
            case "finishedsets":
                SingleInfo = PlayerList.GetWhoPlayer();
                var list = await GetSetInfoAsync(content);
                await FinishManualProcessingAsync(list);
                await ContinueTurnAsync(); //try this
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
            if (SingleInfo!.MainHandList.Count > 10)
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
            await ProcessEndAsync();
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
            {
                await EndTurnAsync();
                return;
            }
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
        //if (SingleInfo!.MainHandList.Any(x => x.WhatCard == EnumCardType.IsChance) == false)
        //{
        //    var newScore = CalculateScore(WhoTurn, true, out DeckRegularDict<MonopolyCardGameCardInformation> newGroup);
        //    SaveRoot!.GameStatus = EnumWhatStatus.LookOnly;
        //    await FinalProcessAsync(newGroup, newScore);
        //    return;
        //}
        SaveRoot.GameStatus = EnumWhatStatus.Other;
        SaveRoot.ManuelStatus = EnumManuelStatus.WentOutAfterDrawing5Cards;
        //_model!.Status
        //SaveRoot.GameStatus = EnumWhatStatus.ManuallyFigureOutMonopolies;
        StartProcessAfterDrawing5Cards();
        await ComputerTurnAsync();
        //rethinking is now required.
    }
    public void PopulateManuelCards()
    {
        //SingleInfo!.TempHands = SingleInfo.MainHandList.Where(x => x.WhatCard != EnumCardType.IsMr && x.WhatCard != EnumCardType.IsGo).ToRegularDeckDict();
        //_model.TempHand1.HandList = SingleInfo.TempHands;
        SingleInfo!.TempSets.Clear();

        var firstList = SingleInfo!.MainHandList.Where(x => x.WhatCard != EnumCardType.IsMr && x.WhatCard != EnumCardType.IsGo).ToRegularDeckDict();
        firstList.ForEach(x => x.WasAutomated = false);

        var tempList = firstList.Where(x => x.WhatCard != EnumCardType.IsChance && x.WhatCard != EnumCardType.IsHouse && x.WhatCard != EnumCardType.IsHotel);

        var groups = tempList.GroupBy(x => x.WhatCard);

        BasicList<DeckRegularDict<MonopolyCardGameCardInformation>> fins = [];
        //BasicList<MonopolyCardGameCardInformation> others = [];
        //BasicList<MonopolyCardGameCardInformation> fins;
        foreach (var item in groups)
        {
            if (item.Key == EnumCardType.IsRailRoad && item.Count() > 1)
            {
                fins.Add(item.ToRegularDeckDict());
                continue;
            }
            if (item.Key == EnumCardType.IsUtilities && item.Count()  == 2)
            {
                fins.Add(item.ToRegularDeckDict());
                continue;
            }
            var card = item.First();
            if (card.Money == 50 || card.Money == 400)
            {
                if (item.Count() == 2)
                {
                    fins.Add(item.ToRegularDeckDict());
                    continue;
                }
            }
            if (item.Count() == 3)
            {
                fins.Add(item.ToRegularDeckDict());
            }
        }
        foreach (var firsts in fins)
        {
            foreach (var item in firsts)
            {
                firstList.RemoveSpecificItem(item); //because should be added to tempsets.
                item.WasAutomated = true; //this means cannot be selected.  but can still show the values though.
                SingleInfo.TempSets.Add(item);
            }
        }
        SingleInfo.TempHands = firstList;
        _model.TempHand1.HandList = SingleInfo.TempHands;
        _model.TempSets1.ClearBoard();
        int x = 0;
        foreach (var firsts in fins)
        {
            x++;
            _model.TempSets1.AddCards(x, firsts);
        }
    }

    private void StartProcessAfterDrawing5Cards()
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.Status = "Needs to manually figure out the monopolies for going out.";
            PopulateManuelCards();   
        }
        else
        {
            _model.Status = $"Waiting for {SingleInfo.NickName} to manually figure out the monopolies for going out.";
        }
        _model.TempSets1.ClearBoard(); //i think we need to clear out the board at this point.
    }
    public void SortTempHand(DeckRegularDict<MonopolyCardGameCardInformation> list)
    {
        SingleInfo!.TempHands.AddRange(list);
        SortCards(SingleInfo.TempHands);
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
        tempList.ForEach(thisPlayer => CreateTradePile(thisPlayer));
    }
    private async Task ProcessEndAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
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
    public bool CanGoOut(IDeckDict<MonopolyCardGameCardInformation> whatGroup, bool onlyOne = false)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        var tempCol = whatGroup.ToRegularDeckDict();
        var groupList = tempCol.GroupBy(items => items.Group).ToBasicList();
        bool hasRailRoad = whatGroup.Any(items => items.WhatCard == EnumCardType.IsRailRoad);
        bool hasUtilities = whatGroup.Any(items => items.WhatCard == EnumCardType.IsUtilities);
        int numWilds = whatGroup.Count(items => items.WhatCard == EnumCardType.IsChance);
        if (hasRailRoad && hasUtilities && onlyOne)
        {
            return false; //this means you cannot have both railroads and utilties.
        }
        if (numWilds < 2 && hasRailRoad == false && hasUtilities == false && groupList.Count == 0)
        {
            return false; //cannot go out because do not have any properties, no railroads, no utilities, and not enough wilds
        }
        var temps = tempCol.Where(items => (int)items.WhatCard > 3 && (int)items.WhatCard < 7).ToRegularDeckDict();
        tempCol.RemoveGivenList(temps);
        BasicList<int> setList = [];
        DeckRegularDict<MonopolyCardGameCardInformation> monCol;
        foreach (var thisGroup in groupList)
        {
            if (thisGroup.Key > 0)
            {
                monCol = MonopolyCol(tempCol, thisGroup.Key, EnumCardType.IsProperty);
                if (monCol.Count == 0)
                {
                    return false;
                }
                setList.Add(thisGroup.Key);
            }
        }
        if (hasRailRoad && setList.Count > 0 && onlyOne)
        {
            return false;
        }
        if (hasUtilities && setList.Count > 0 && onlyOne)
        {
            return false;
        }
        if (setList.Count > 1 && onlyOne)
        {
            return false;
        }
        if (hasRailRoad)
        {
            monCol = MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
        }
        if (hasUtilities)
        {
            monCol = MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
        }
        setList.ForEach(thisSet =>
        {
            HouseCollection(tempCol); //to filter further
        });
        tempCol.RemoveAllOnly(items => items.WhatCard == EnumCardType.IsChance);
        return tempCol.Count == 0;
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
    internal static DeckRegularDict<MonopolyCardGameCardInformation> MonopolyColWild(DeckRegularDict<MonopolyCardGameCardInformation> whatCol, int whatGroup, EnumCardType whatType, bool useWild)
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
            if (howMany == 2 || howMany == 1 && numWilds > 0 && useWild == true)
            {
                output.AddRange(temps);
                whatCol.RemoveGivenList(temps);
                if (howMany == 2)
                {
                    return output;
                }
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
            if (howMany > 1 || howMany == 1 && numWilds > 0 && useWild == true)
            {
                output.AddRange(temps);
                whatCol.RemoveGivenList(temps);
                if (numWilds == 0)
                {
                    return output;
                }
            }
            if (numWilds > 0 && useWild) //can only use one wild for rail roads
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
        if (useWild == false)
        {
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
    internal static DeckRegularDict<MonopolyCardGameCardInformation> HouseCollectionWild(DeckRegularDict<MonopolyCardGameCardInformation> whatCol, bool makeWildHotel, bool useWildHouse)
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
                if (!whatCol.Any(items => items.WhatCard == EnumCardType.IsChance) || useWildHouse == false)
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
        else if (!whatCol.Any(items => items.WhatCard == EnumCardType.IsChance) || makeWildHotel == false)
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
    private static ListInfo PlaceTokens(BasicList<ListInfo> whatGroup)
    {
        ListInfo output;
        decimal mostss = 0;
        output = new ListInfo();
        whatGroup.ForEach(thisList =>
        {
            decimal currentMoneys = CalculateMoneyFromGroup(thisList, 0);
            if (currentMoneys > mostss)
            {
                output = thisList;
                mostss = currentMoneys;
            }
        });
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
                bases *= (thisList.NumberOfHouses + 1);
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
        tempScore += (thisCol.Count * 200);
        newGroup.AddRange(thisCol);
        thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsToken).ToRegularDeckDict();
        var tokenList = thisCol.ToRegularDeckDict();
        int tokens = thisCol.Count;
        ListInfo thisList;
        ListInfo places;
        BasicList<ListInfo> listMons = [];
        DeckRegularDict<MonopolyCardGameCardInformation> mons;
        DeckRegularDict<MonopolyCardGameCardInformation> hou;
        var possList = tempCol.Where(items => items.Group > 0).GroupOrderDescending(items => items.Group).ToBasicList();
        if (tempCol.Any(items => items.WhatCard == EnumCardType.IsChance) == false)
        {//processes for when there is no chance (easiest).
            mons = MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
            if (mons.Count > 0)
            {
                newGroup.AddRange(mons);
                thisList = new()
                {
                    RailRoads = mons.Count,
                    WhatCard = EnumCardType.IsRailRoad,
                    ID = listMons.Count + 1
                };
                listMons.Add(thisList);
            }
            mons = MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
            if (mons.Count > 0)
            {
                newGroup.AddRange(mons);
                thisList = new();
                thisList.WhatCard = EnumCardType.IsUtilities;
                thisList.ID = listMons.Count + 1;
                listMons.Add(thisList);
            }
            DeckRegularDict<MonopolyCardGameCardInformation> prList = [];
            possList.ForConditionalItems(items => items.Key > 0, thisPoss =>
            {
                mons = MonopolyCol(tempCol, thisPoss.Key, EnumCardType.IsProperty);
                if (mons.Count > 0)
                {
                    prList.AddRange(mons);
                    thisList = new();
                    thisList.Group = thisPoss.Key;
                    hou = HouseCollection(tempCol);
                    if (hou.Count > 0)
                    {
                        prList.AddRange(hou);
                    }
                    if (hou.Count == 5)
                    {
                        thisList.HasHotel = true;
                        thisList.NumberOfHouses = hou.Count - 1;
                    }
                    else
                    {
                        thisList.HasHotel = false;
                        thisList.NumberOfHouses = hou.Count;
                    }
                    thisList.ID = listMons.Count + 1;
                    listMons.Add(thisList);
                }
            });
            newGroup.AddRange(prList);
            if (listMons.Count == 0)
            {
                return tempScore;
            }
            newGroup.AddRange(tokenList);
            places = PlaceTokens(listMons);
            decimal temps;
            listMons.ForEach(thisItem =>
            {
                if (thisItem.ID == places.ID)
                {
                    temps = CalculateMoneyFromGroup(thisItem, tokens);
                }
                else
                {
                    temps = CalculateMoneyFromGroup(thisItem, 0);
                }
                tempScore += temps;
                if (thisItem.WhatCard == EnumCardType.IsUtilities && temps == 0)
                {
                    throw new CustomBasicException("Utilities cannot worth 0 points");
                }
            });
            return tempScore;
        }
        // Here is the new logic for the wildcards.  Given this table:
        // Number of houses
        // Group  Value   1       2       3       4       Hotel
        // 8      400     800     1200    1600    2000    2500
        // 7      350     700     1050    1400    1750    2250
        // 6      300     600     900     1200    1500    2000
        // 5      250     500     750     1000    1250    1750
        // 4      200     400     600     800     1000    1500
        // 3      150     300     450     600     750     1250
        // 2      100     200     300     400     500     1000
        // 1      50      100     150     200     250     750

        // 2 RR   250
        // 2 util 500
        // 3 RR   500
        // 4 RR   1000

        // Now the logic is as follows:  it is always better to be a token than an extra house because
        // for any value in the table, unless you have 2 or more token cards.  We set a variable
        // to only create wild houses when we have 2 or more token cards
        // The best way to see this is to assume that you have 1 token and 1 chance card and try the
        // values in the table to see how that affects the final score.
        // An example:  You have group 5 with 3 houses.  This is worth $1000.  A token card means you have
        // $2000 in value.  Now, with a wild card, you can either add a house, making it $1250 or $2500 with
        // token, or add an aditional token, making it $1000+$1000+$1000 or $3000, which is the better choice.
        // This holds true for the entire table, where being a token is always a break even or better move, with
        // the only exception being that if your group is 1, 2, or 3, and you have 4 houses, you would be better
        // off being a wild hotel card than a token card.  This is because the $500 value of the hotel is so
        // much greater than the base property value in the table.

        // Sometimes it is better to be a railroad or a utility though.   If the value given by a railroad or a utility
        // is greater than the value in the chart where you are then you should do this.  This means for two railroads
        // worth $250 you should always take it if you are group 3 with no houses, group 2 with 1 or no houses, or
        // group 1 with 0, 1, 2, or 3 houses.  The logic is similar for 3 railroads and 2 utilities, or 4 railroads,
        // which you can see in the case select code below.

        // First, we need to know how many of everything we have.  Not all these values a used, but they are taken
        // in case we discover a case where we need to use them
        int monoCount, monoWildCount, rrCount, utilCount, houseCount, hotelCount, wildCount, tokenCount, houseWildCount;
        DeckRegularDict<MonopolyCardGameCardInformation> propList = [];
        DeckRegularDict<MonopolyCardGameCardInformation> propCheckList;
        DeckRegularDict<MonopolyCardGameCardInformation> houseList;
        DeckRegularDict<MonopolyCardGameCardInformation> houseCheckList;
        BasicList<int> searchPos = [];
        rrCount = 0;
        utilCount = 0;
        houseCount = 0;
        hotelCount = 0;
        wildCount = 0;
        tokenCount = 0;
        houseWildCount = 0;
        monoWildCount = 0;
        monoCount = 0;
        int i;
        thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsProperty).ToRegularDeckDict();
        propList.AddRange(thisCol);
        possList = tempCol.Where(items => items.Group > 0).GroupOrderAscending(items => items.Group).ToBasicList();
        possList.ForEach(thisPoss =>
        {
            propCheckList = MonopolyCol(propList, thisPoss.Key, EnumCardType.IsProperty);
            if (propCheckList.Count > 0)
            {
                monoCount += 1;
            }
        });
        thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsProperty).ToRegularDeckDict();
        propList.ReplaceRange(thisCol);
        thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsChance).ToRegularDeckDict();
        propList.AddRange(thisCol);
        possList = SingleInfo.MainHandList.Where(items => items.Group > 0).GroupOrderAscending(items => items.Group).ToBasicList();
        int numCards;
        foreach (var thisItem in possList)
        {
            propCheckList = MonopolyCol(propList, thisItem.Key, EnumCardType.IsProperty);
            if (propCheckList.Count > 0)
            {
                monoWildCount++;
                numCards = 0;
                if (thisItem.Key != 0)
                {
                    var loopTo1 = propCheckList.Count;
                    for (i = 1; i <= loopTo1; i++)
                    {
                        if (propCheckList[i - 1].WhatCard != EnumCardType.IsChance)
                        {
                            numCards++;
                        }
                    }
                    searchPos.Add(thisItem.Key);
                    searchPos.Add(propCheckList.Count - numCards);
                }
            }
        };
        tokenCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsToken);
        rrCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsRailRoad);
        utilCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsUtilities);
        hotelCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsHotel);
        wildCount = tempCol.Count(items => items.WhatCard == EnumCardType.IsChance);
        thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsHouse).ToRegularDeckDict();
        houseList = [.. thisCol];
        houseCheckList = HouseCollection(houseList);
        thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsHouse || items.WhatCard == EnumCardType.IsChance).ToRegularDeckDict();
        houseList.ReplaceRange(thisCol);
        if (houseList.Count > 0)
        {
            houseCheckList = HouseCollection(houseList);
            houseWildCount = houseCheckList.Count;
        }
        int monGroup;
        bool wildRail = false;
        bool wildUtil = false;
        bool wildHouse = false;
        bool wildHotel = false;
        bool wildProp = false;
        bool largeProp = false;
        if (monoWildCount > monoCount) // This loop is to check all the conditions and see what is best to make wild
        {
            wildProp = true;
        }
        possList = tempCol.Where(items => items.Group > 0).GroupOrderDescending(items => items.Group).ToBasicList();
        possList.ForEach(thisItem =>
        {
            monGroup = thisItem.Key;
            switch (monGroup)// Here is the logic based on the table above
            {
                case 8:
                case 7:
                    {
                        if ((rrCount == 2) & (houseCount == 0))
                        {
                            wildRail = true;
                        }
                        if ((rrCount == 3) & (houseCount < 2))
                        {
                            wildRail = true;
                        }
                        if (((houseCount >= 2) & (rrCount == 3)) | ((houseCount > 0) & (rrCount == 2)))
                        {
                            largeProp = true;
                        }
                        if ((utilCount == 1) & (houseCount == 0))
                        {
                            wildUtil = true;
                        }
                        break;
                    }
                case 6:
                case 5:
                    {
                        if ((rrCount == 2) & (houseCount == 0))
                        {
                            wildRail = true;
                        }
                        if ((rrCount == 3) & (houseCount < 3))
                        {
                            wildRail = true;
                        }
                        if (((houseCount >= 3) & (rrCount == 3)) | ((houseCount > 0) & (rrCount == 2)))
                        {
                            largeProp = true;
                        }
                        if ((utilCount == 1) & (houseCount == 0))
                        {
                            wildUtil = true;
                        }
                        break;
                    }
                case 4:
                    {
                        if ((rrCount == 2) & (houseCount < 2))
                        {
                            wildRail = true;
                        }
                        if ((rrCount == 3) & (houseCount < 4))
                        {
                            wildRail = true;
                        }
                        if (((houseCount >= 4) & (rrCount == 3)) | ((houseCount >= 2) & (rrCount == 2)))
                        {
                            largeProp = true;
                        }
                        if ((utilCount == 1) & (houseCount < 2))
                        {
                            wildUtil = true;
                        }
                        if (houseCount == 4)
                        {
                            wildHotel = true;
                        }
                        break;
                    }
                case 3:
                    {
                        if ((rrCount == 1) & (houseCount == 0))
                        {
                            wildRail = true;
                        }
                        if ((rrCount == 2) & (houseCount < 3))
                        {
                            wildRail = true;
                        }
                        if ((rrCount == 3) & (houseCount < 5))
                        {
                            wildRail = true;
                        }
                        if (((houseCount >= 5) & (rrCount == 3)) | ((houseCount >= 3) & (rrCount == 2)) | ((houseCount == 0) & (rrCount == 1)))
                        {
                            largeProp = true;
                        }
                        if ((utilCount == 1) & (houseCount < 3))
                        {
                            wildUtil = true;
                        }
                        if (houseCount == 4)
                        {
                            wildHotel = true;
                        }
                        break;
                    }
                case 2:
                    {
                        if ((rrCount == 1) & (houseCount < 2))
                        {
                            wildRail = true;
                        }
                        if ((rrCount == 2) & (houseCount < 4))
                        {
                            wildRail = true;
                        }
                        if ((rrCount == 3) & (houseCount < 5))
                        {
                            wildRail = true;
                        }
                        if (((houseCount >= 5) & (rrCount == 3)) | ((houseCount >= 4) & (rrCount == 2)) | ((houseCount >= 2) & (rrCount == 1)))
                        {
                            largeProp = true;
                        }
                        if ((utilCount == 1) & (houseCount < 4))
                        {
                            wildUtil = true;
                        }
                        if (houseCount == 4)
                        {
                            wildHotel = true;
                        }
                        break;
                    }
                case 1:
                    {
                        if ((rrCount == 1) & (houseCount < 4))
                        {
                            wildRail = true;
                        }
                        if ((rrCount == 2) & (houseCount < 5))
                        {
                            wildRail = true;
                        }
                        if ((rrCount == 3) & (houseCount < 5))
                        {
                            wildRail = true;
                        }
                        if (((houseCount >= 5) & (rrCount == 3)) | ((houseCount >= 5) & (rrCount == 2)) | ((houseCount >= 4) & (rrCount == 1)))
                        {
                            largeProp = true;
                        }
                        if ((utilCount == 1) & (houseCount < 5))
                        {
                            wildUtil = true;
                        }
                        if (houseCount == 4)
                        {
                            wildHotel = true;
                        }
                        break;
                    }
            }
        });
        // now we've completed our first pass and can start using the wild cards
        // if we need a wild railroad card, use it first
        if (largeProp)
        {
            wildRail = false;
            wildUtil = false;
        }
        if ((wildRail))
        {
            mons = MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
            if (mons.Count > 0)
            {
                newGroup.AddRange(mons);
                thisList = new();
                thisList.RailRoads = mons.Count;
                thisList.WhatCard = EnumCardType.IsRailRoad;
                thisList.ID = listMons.Count + 1;
                listMons.Add(thisList);
            }
        }
        // if we need a wild utility card, use it next
        if ((wildUtil))
        {
            mons = MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
            if (mons.Count > 0)
            {
                newGroup.AddRange(mons);
                thisList = new();
                thisList.WhatCard = EnumCardType.IsUtilities;
                thisList.ID = listMons.Count + 1;
                listMons.Add(thisList);
            }
        }

        var loopTo2 = searchPos.Count;
        for (var x = 1; x <= loopTo2; x += 2)
        {
            mons = MonopolyColWild(tempCol, searchPos[x - 1], EnumCardType.IsProperty, wildProp);
            if (mons.Count > 0)
            {
                foreach (var card in mons)
                {
                    newGroup.Add(card);
                }
                thisList = new();
                thisList.Group = searchPos[x - 1]; // because 0 based
                if (thisList.Group == 0)
                {
                    throw new CustomBasicException("The group cannot be 0 for properties");
                }
                // call new function that will handle wild houses or wild hotels
                hou = HouseCollectionWild(tempCol, wildHotel, wildHouse);
                if (hou.Count == 5)
                {
                    thisList.HasHotel = true;
                    thisList.NumberOfHouses = 4;
                }
                else
                {
                    thisList.HasHotel = false;
                    thisList.NumberOfHouses = hou.Count;
                }
                thisList.ID = listMons.Count + 1;
                if (hou.Count > 0)
                {
                    foreach (var card in hou)
                    {
                        newGroup.Add(card);
                    }

                }
                listMons.Add(thisList);
            }
        }
        // okay, at this point, any wild cards left over will be tokens
        if (listMons.Count == 0)
        {
            return tempScore;
        }
        foreach (var card in tokenList)
        {
            newGroup.Add(card);
        }
        places = PlaceTokens(listMons);
        thisCol = tempCol.Where(items => items.WhatCard == EnumCardType.IsChance).ToRegularDeckDict();
        int manys = thisCol.Count;
        newGroup.AddRange(thisCol);
        foreach (var card in thisCol)
        {
            tempCol.RemoveSpecificItem(card);
        }
        // if we have utilities or railroads without wild cards, we need to add them in the score now
        // if we already did them with wild cards, we won't find any left
        mons = MonopolyCol(tempCol, 0, EnumCardType.IsUtilities);
        if (mons.Count > 0)
        {
            newGroup.AddRange(mons);
            thisList = new()
            {
                WhatCard = EnumCardType.IsUtilities,
                ID = listMons.Count + 1
            };
            listMons.Add(thisList);
        }
        mons = MonopolyCol(tempCol, 0, EnumCardType.IsRailRoad);
        if (mons.Count > 0)
        {
            newGroup.AddRange(mons);
            thisList = new()
            {
                WhatCard = EnumCardType.IsRailRoad,
                ID = listMons.Count + 1
            };
            listMons.Add(thisList);
        }
        tokens += manys;
        foreach (var item in listMons)
        {
            if (item.ID == places.ID)
            {
                tempScore += CalculateMoneyFromGroup(item, tokens);
            }
            else
            {
                var argNumTokens = 0;
                tempScore += CalculateMoneyFromGroup(item, argNumTokens);
            }
        }
        return tempScore;
    }
    #endregion
    #region "ManuelSetProcesses"
    public bool HasAllValidMonopolies()
    {
        bool usedChance = false;
        for (int x = 1; x <= _model.TempSets1.HowManySets; x++)
        {
            var list = WhatSet(x);
            if (list.Any(x => x.WhatCard == EnumCardType.IsChance))
            {
                usedChance = true;
            }
            if (list.Count > 0)
            {
                if (CanGoOut(list, true) == false)
                {
                    return false;
                }
            }
        }
        if (usedChance == false)
        {
            return false; //because you have to use at least one chance card.
        }
        return true;
    }
    public IDeckDict<MonopolyCardGameCardInformation> WhatSet(int whichOne)
    {
        return _model!.TempSets1!.ObjectList(whichOne);
    }
    //go ahead and make public.  can do private late.
    //private decimal CalculateScore()
    //{
    //    decimal score = 0;
    //    for (int x = 1; x <= _model.TempSets1.HowManySets; x++)
    //    {
    //        var list = WhatSet(x);
    //        if (list.Count > 0)
    //        {
    //            score += CalculateScore(list);
    //        }
    //    }
    //    //has to start taking risks now.
    //    return score;
    //}
    public BasicList<TempInfo> ListValidSets()
    {
        BasicList<TempInfo> output = [];
        for (int x = 1; x <= _model.TempSets1.HowManySets; x++)
        {
            var list = WhatSet(x);
            if (CanGoOut(list, true))
            {
                output.Add(new TempInfo()
                {
                    CardList= list.ToRegularDeckDict(),
                    SetNumber = x
                });
            }
        }
        return output;
    }

    private static decimal CalculateScore(IDeckDict<MonopolyCardGameCardInformation> list)
    {
        //this means it all has to match.
        //unfortunately, lots of copy/paste though.
        var tokenList = list.Where(x => x.WhatCard == EnumCardType.IsToken).ToRegularDeckDict();
        int tokens = tokenList.Count;


        BasicList<ListInfo> listMons = [];
        //DeckRegularDict<MonopolyCardGameCardInformation> hou;
        var possList = list.Where(items => items.Group > 0).GroupOrderDescending(items => items.Group).ToBasicList();

        //bool isRailRoad = list.Any(x => x.WhatCard == EnumCardType.IsRailRoad);
        //bool isUtility = list.Any(x => x.WhatCard == EnumCardType.IsUtilities);
        int rawRailRoads = list.Count(x => x.WhatCard == EnumCardType.IsRailRoad);
        bool hasUtilities = list.Any(x => x.WhatCard == EnumCardType.IsUtilities);
        //int rawUtilitys = list.Count(x => x.WhatCard == EnumCardType.IsUtilities);
        //do utilities and railroads first.
        int chances = list.Count(x => x.WhatCard == EnumCardType.IsChance);
        int totalChances = chances;
        //temps = CalculateMoneyFromGroup(thisItem, tokens);
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
                chances --;
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
        //temps = CalculateMoneyFromGroup(thisItem, tokens);
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
        decimal finalScore = CalculateScore(WhoTurn, true, out DeckRegularDict<MonopolyCardGameCardInformation> lasts);
        decimal totalScore = score + finalScore;
        mans.AddRange(lasts);
        await FinalProcessAsync(mans, totalScore);
    }
    #endregion
}