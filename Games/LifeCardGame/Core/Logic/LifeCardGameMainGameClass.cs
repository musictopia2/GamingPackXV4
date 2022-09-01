namespace LifeCardGame.Core.Logic;
[SingletonGame]
public class LifeCardGameMainGameClass
    : CardGameClass<LifeCardGameCardInformation, LifeCardGamePlayerItem, LifeCardGameSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly LifeCardGameVMData _model;
    private readonly CommandContainer _command;
    private readonly LifeCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    public LifeCardGameMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        LifeCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<LifeCardGameCardInformation> cardInfo,
        CommandContainer command,
        LifeCardGameGameContainer gameContainer,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
        _gameContainer.ChosePlayerAsync = ChosePlayerAsync;
    }
    public int OtherTurn
    {
        get
        {
            return SaveRoot!.PlayOrder.OtherTurn;
        }
        set
        {
            SaveRoot!.PlayOrder.OtherTurn = value;
        }
    }
    public override Task ContinueTurnAsync()
    {
        PlayerList.ForEach(player => player.LifeStory!.RefreshCards()); //try this way
        return base.ContinueTurnAsync();
    }
    private int _otherPlayerDraws;
    public override async Task FinishGetSavedAsync()
    {
        CreateLifeStories(); //even for autoresume needs to be done now.  if autoresume, can later resume it.
        await PlayerList!.ForEachAsync(async thisPlayer =>
        {
            var thisList = await js.DeserializeObjectAsync<DeckRegularDict<LifeCardGameCardInformation>>(thisPlayer.LifeString);
            thisPlayer.LifeStory!.HandList = new DeckRegularDict<LifeCardGameCardInformation>(thisList);
        });
        await PlayerList.RepositionCardsAsync(this, _gameContainer, _model); //i think.
        await base.FinishGetSavedAsync();
    }
    private void CreateLifeStories()
    {
        PlayerList!.ForEach(thisPlayer => _gameContainer.CreateLifeStoryPile(_model, thisPlayer));
    }
    public override async Task PopulateSaveRootAsync()
    {
        await PlayerList!.ForEachAsync(async thisPlayer =>
        {
            thisPlayer.LifeString = await js.SerializeObjectAsync(thisPlayer.LifeStory!.HandList);
        });

        await base.PopulateSaveRootAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (isBeginning)
        {
            CreateLifeStories();
        }
        else
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.LifeStory!.HandList.Clear());
        }
        SaveRoot!.ImmediatelyStartTurn = true;
        SaveRoot.YearList.Clear();
        PlayerList!.ForEach(thisPlayer => thisPlayer.Points = 0);
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        if (PlayerList!.HasYears())
        {
            throw new CustomBasicException("Players cannot have years");
        }
        _model!.Pile1!.ClearCards();
        _model.CurrentPile!.ClearCards();
        SaveRoot!.CurrentCard = 0;
        var newList = _gameContainer.YearCards();
        _model.Deck1!.ShuffleInExtraCards(newList);
        var tempList = _model.Deck1.DeckList().ToRegularDeckDict();
        if (tempList.Any(items => items.CanBeInPlayerHandToBeginWith == false) == false)
        {
            throw new CustomBasicException("There was no years passed in the entire deck.  Really rethink");
        }
        if (tempList.Count(items => items.CanBeInPlayerHandToBeginWith == false) != 8)
        {
            throw new CustomBasicException("Must have 6 years passed in the deck.  Rethink");
        }
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        LifeCardGameCardInformation thisCard;
        switch (status)
        {
            case "turnbacktime":
                DeckRegularDict<LifeCardGameCardInformation> thisList = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                await FinishedTurningBackTimeAsync(thisList);
                return;
            case "playcard":
                thisCard = SingleInfo!.MainHandList.GetSpecificItem(int.Parse(content));
                await PlayCardAsync(thisCard);
                return;
            case "choseplayer":
                await ChosePlayerAsync(int.Parse(content));
                return;
            case "cardchosen":
                thisCard = _gameContainer.DeckList!.GetSpecificItem(int.Parse(content));
                await ChoseSingleCardAsync(thisCard);
                return;
            case "cardstraded":
                var thisTrade = await js.DeserializeObjectAsync<TradeCard>(content);
                var yourCard = _gameContainer.DeckList!.GetSpecificItem(thisTrade.YourCard);
                var opponentCard = _gameContainer.DeckList.GetSpecificItem(thisTrade.OtherCard);
                await TradeCardsAsync(yourCard, opponentCard);
                return;
            case "lifeswap":
                var thisSwap = await js.DeserializeObjectAsync<Swap>(content);
                await FinishLifeSwapAsync(thisSwap.Player, thisSwap.YourCards, thisSwap.OtherCards);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        _model!.CurrentPile!.ClearCards();
        SingleInfo = PlayerList!.GetWhoPlayer();
        await PlayerList.RepositionCardsAsync(this, _gameContainer, _model);
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
        await StartNewTurnAsync();
    }
    protected override void GetPlayerToContinueTurn()
    {
        if (OtherTurn > 0)
        {
            SingleInfo = PlayerList!.GetOtherPlayer();
        }
        else
        {
            base.GetPlayerToContinueTurn();
        }
    }
    public override async Task DiscardAsync(LifeCardGameCardInformation ThisCard)
    {
        if (SingleInfo!.MainHandList.Count == 5)
        {
            SingleInfo.MainHandList.RemoveObjectByDeck(ThisCard.Deck);
        }
        await AnimatePlayAsync(ThisCard);
        await DrawAtLastAsync();
    }
    private async Task DrawAtLastAsync()
    {
        LifeCardGamePlayerItem thisPlayer;
        if (_otherPlayerDraws > 0)
        {
            thisPlayer = PlayerList![_otherPlayerDraws];
        }
        else
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            thisPlayer = SingleInfo;
        }
        CheckDeck();
        SingleInfo!.Points = SingleInfo.LifeStory!.TotalPoints();
        OtherTurn = 0;
        _model!.CurrentPile!.ClearCards();
        if (SaveRoot!.YearList.Count == 6)
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.Points).First();
            await ShowWinAsync();
            return;
        }
        thisPlayer.MainHandList.Add(_model.Deck1!.DrawCard());
        if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
        {
            thisPlayer.MainHandList.Sort(); //i think this should be fine.
        }
        await EndTurnAsync();
    }
    private void CheckDeck()
    {
        int oldDeck = 0;
        do
        {
            var nextCard = _model!.Deck1!.RevealCard();
            if (nextCard.Points > 0)
            {
                return;
            }
            nextCard = _model.Deck1.DrawCard();
            if (nextCard.Points > 0)
            {
                throw new CustomBasicException("Can't have 0 points");
            }
            if (oldDeck == nextCard.Deck)
            {
                throw new CustomBasicException("A card is repeating");
            }
            oldDeck = nextCard.Deck;
            SaveRoot!.YearList.Add(nextCard.Deck);
            _toast.ShowInfoToast($"Time's Flying.  Total years is {SaveRoot.YearsPassed()}.  Therefore; another card is being drawn.");
            if (SaveRoot.YearList.Count == 6)
            {
                return;
            }
        } while (true);
    }
    private async Task FinishedTurningBackTimeAsync(IEnumerableDeck<LifeCardGameCardInformation> thisList) //i think we need this parameter now.
    {
        _model!.Deck1!.OriginalList(thisList);
        _model.CurrentPile!.ClearCards();
        await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
        await DrawAtLastAsync();
    }
    private async Task TurnBackTimeAsync()
    {
        _toast.ShowInfoToast("Turn back time.  Therefore; the deck is being reshuffled");
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(SaveRoot!.YearList.Last());
        SaveRoot.YearList.RemoveLastItem(); //i think.
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
        {
            Network!.IsEnabled = true;
            return;
        }
        var thisList = _model.Deck1!.DeckList();
        thisList.Add(thisCard);
        thisList.ShuffleList();
        if (BasicData!.MultiPlayer)
        {
            await Network!.SendAllAsync("turnbacktime", thisList.Select(items => items.Deck).ToBasicList());
        }
        await FinishedTurningBackTimeAsync(thisList);
    }
    private void RecalculatePoints(LifeCardGamePlayerItem otherPlayer)
    {
        var thisCard = _model!.CurrentPile!.GetCardInfo();
        if (thisCard.OpponentKeepsCard)
        {
            otherPlayer.LifeStory!.AddCard(thisCard);
        }
        otherPlayer.Points = otherPlayer.LifeStory!.TotalPoints();
    }
    private async Task ShowOtherPlayerAsync(LifeCardGamePlayerItem otherPlayer)
    {
        _gameContainer!.PlayerChosen = otherPlayer.Id;
        _gameContainer.CardChosen = null;
        await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        _gameContainer.PlayerChosen = 0;
        await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
    }
    private async Task DiscardPassportAsync(LifeCardGamePlayerItem otherPlayer)
    {
        var thisCard = otherPlayer.LifeStory!.HandList.Single(items => items.SpecialCategory == EnumSpecialCardCategory.Passport);
        otherPlayer.LifeStory.RemoveCard(thisCard);
        RecalculatePoints(otherPlayer);
        await AnimatePlayAsync(thisCard);
        await DrawAtLastAsync();
    }
    private async Task TakePaydayAsync(LifeCardGamePlayerItem otherPlayer)
    {
        var thisCard = otherPlayer.LifeStory!.HandList.First(items => items.IsPayday() == true);
        otherPlayer.LifeStory.RemoveCard(thisCard);
        RecalculatePoints(otherPlayer);
        SingleInfo!.LifeStory!.AddCard(thisCard);
        SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
        await DrawAtLastAsync();
    }
    private async Task DiscardPaydayAsync(LifeCardGamePlayerItem otherPlayer)
    {
        var thisCard = otherPlayer.LifeStory!.HandList.First(items => items.IsPayday() == true);
        otherPlayer.LifeStory.RemoveCard(thisCard);
        RecalculatePoints(otherPlayer);
        await AnimatePlayAsync(thisCard);
        await DrawAtLastAsync();
    }
    private async Task TakeCardAsync(LifeCardGameCardInformation thisCard)
    {
        var tempPlayer = _gameContainer!.PlayerWithCard(thisCard);
        await ShowOtherCardAsync(tempPlayer, thisCard);
        tempPlayer.LifeStory!.RemoveCard(thisCard);
        RecalculatePoints(tempPlayer);
        SingleInfo!.LifeStory!.AddCard(thisCard);
        SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
        await DrawAtLastAsync();
    }
    private async Task ShowOtherCardAsync(LifeCardGamePlayerItem tempPlayer, LifeCardGameCardInformation thisCard)
    {
        _gameContainer!.PlayerChosen = tempPlayer.Id;
        thisCard.IsSelected = false;
        _gameContainer.CardChosen = thisCard;
        await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        _gameContainer.PlayerChosen = 0;
        _gameContainer.CardChosen = null;
        await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
    }
    private async Task DonateToCharityAsync(LifeCardGameCardInformation thisCard)
    {
        var tempPlayer = _gameContainer!.PlayerWithCard(thisCard);
        await ShowOtherCardAsync(tempPlayer, thisCard);
        tempPlayer.LifeStory!.RemoveCard(thisCard);
        RecalculatePoints(tempPlayer);
        await AnimatePlayAsync(thisCard);
        await DrawAtLastAsync();
    }
    private async Task FinishLifeSwapAsync(int otherPlayer, BasicList<int> yourList, BasicList<int> otherList)
    {
        var tempPlayer = PlayerList![otherPlayer];
        var opponentCardList = otherList.Select(items => _gameContainer.DeckList!.GetSpecificItem(items)).ToRegularDeckDict();
        var yourCardList = yourList.Select(items => _gameContainer.DeckList!.GetSpecificItem(items)).ToRegularDeckDict();
        yourList.ForEach(index => SingleInfo!.MainHandList.RemoveObjectByDeck(index));
        otherList.ForEach(index => tempPlayer.MainHandList.RemoveObjectByDeck(index));
        SingleInfo!.MainHandList.AddRange(opponentCardList);
        tempPlayer.MainHandList.AddRange(yourCardList);
        if (tempPlayer.PlayerCategory == EnumPlayerCategory.Self)
        {
            tempPlayer.MainHandList.Sort();
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            SingleInfo.MainHandList.Sort();
        }
        await DrawAtLastAsync();
    }
    private async Task SwapEntireHandAsync(LifeCardGamePlayerItem otherPlayer)
    {
        var yourHand = SingleInfo!.MainHandList.ToRegularDeckDict();
        var opponentHand = otherPlayer.MainHandList.ToRegularDeckDict();
        SingleInfo.MainHandList.ReplaceRange(opponentHand);
        otherPlayer.MainHandList.ReplaceRange(yourHand);
        _otherPlayerDraws = otherPlayer.Id;
        otherPlayer.LifeStory!.AddCard(_model!.CurrentPile!.GetCardInfo());
        if (otherPlayer.PlayerCategory == EnumPlayerCategory.Self)
        {
            otherPlayer.MainHandList.Sort();
        }
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            SingleInfo.MainHandList.Sort();
        }
        await DrawAtLastAsync();
    }
    private async Task LifeSwapAsync(LifeCardGamePlayerItem otherPlayer)
    {
        await ShowOtherPlayerAsync(otherPlayer);
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
        {
            Network!.IsEnabled = true;
            return;
        }
        var yourRandomList = SingleInfo.MainHandList.GetRandomList(false, 2);
        var opponentRandomList = otherPlayer.MainHandList.GetRandomList(false, 2);
        Swap thisS = new();
        thisS.Player = otherPlayer.Id;
        thisS.YourCards = yourRandomList.Select(items => items.Deck).ToBasicList();
        thisS.OtherCards = opponentRandomList.Select(items => items.Deck).ToBasicList();
        if (BasicData!.MultiPlayer)
        {
            await Network!.SendAllAsync("lifeswap", thisS);
        }
        await FinishLifeSwapAsync(otherPlayer.Id, thisS.YourCards, thisS.OtherCards);
    }
    public async Task ChosePlayerAsync(int player)
    {
        if (_model!.CurrentPile!.PileEmpty())
        {
            throw new CustomBasicException("There is no card in current pile");
        }
        var thisCard = _model.CurrentPile.GetCardInfo();
        OtherTurn = player;
        _gameContainer!.PlayerChosen = OtherTurn;
        var otherPlayer = PlayerList!.GetOtherPlayer();
        await PlayerList.RepositionCardsAsync(this, _gameContainer, _model);
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
        _gameContainer.PlayerChosen = 0;
        await PlayerList.RepositionCardsAsync(this, _gameContainer, _model);
        if (thisCard.Action == EnumAction.MidlifeCrisis)
        {
            await SwapEntireHandAsync(otherPlayer);
            return;
        }
        if (thisCard.Action == EnumAction.LifeSwap)
        {
            await LifeSwapAsync(otherPlayer);
            return;
        }
        if (thisCard.Action == EnumAction.LostPassport)
        {
            await DiscardPassportAsync(otherPlayer);
            return;
        }
        if (thisCard.Action == EnumAction.IMTheBoss)
        {
            await TakePaydayAsync(otherPlayer);
            return;
        }
        if (thisCard.Action == EnumAction.YoureFired)
        {
            await DiscardPaydayAsync(otherPlayer);
            return;
        }
        await ContinueTurnAsync();
    }
    public async Task ChoseSingleCardAsync(LifeCardGameCardInformation thisCard)
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        if (_model!.CurrentPile!.PileEmpty())
        {
            throw new CustomBasicException("There is no card in current pile");
        }
        var otherCard = _model.CurrentPile.GetCardInfo();
        switch (otherCard.Action)
        {
            case EnumAction.AdoptBaby:
            case EnumAction.LongLostRelative:
            case EnumAction.Lawsuit:
            case EnumAction.YourStory:
            case EnumAction.SecondChance:
                await TakeCardAsync(thisCard);
                return;
            case EnumAction.DonateToCharity:
                await DonateToCharityAsync(thisCard);
                break;
            default:
                throw new CustomBasicException("Don't know what to do about choose single card now");
        }
    }
    public async Task TradeCardsAsync(LifeCardGameCardInformation yourCard, LifeCardGameCardInformation opponentCard)
    {
        if (_model!.CurrentPile!.PileEmpty())
        {
            throw new CustomBasicException("There is no card in current pile");
        }
        var thisCard = _model.CurrentPile.GetCardInfo();
        if (thisCard.Action == EnumAction.None)
        {
            throw new CustomBasicException("Must have action in order to trade cards");
        }
        if (SingleInfo!.LifeStory!.HandList.ObjectExist(yourCard.Deck) == false)
        {
            throw new CustomBasicException("Don't have the requested card in your life story");
        }
        if (thisCard.Action == EnumAction.CareerSwap || thisCard.Action == EnumAction.MovingHouse || thisCard.Action == EnumAction.MixUpAtVets)
        {
            _gameContainer!.TradeList!.Clear();
            _gameContainer.TradeList.Add(yourCard);
            _gameContainer.TradeList.Add(opponentCard);
            _gameContainer.PlayerChosen = 0;
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
            _command.UpdateAll();
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(1);
            }
            var otherPlayer = _gameContainer.PlayerWithCard(opponentCard);
            otherPlayer.LifeStory!.RemoveCard(opponentCard);
            otherPlayer.LifeStory.AddCard(yourCard);
            SingleInfo.LifeStory.RemoveCard(yourCard);
            SingleInfo.LifeStory.AddCard(opponentCard);
            _gameContainer.TradeList.Clear();
            RecalculatePoints(otherPlayer);
            SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
            await DrawAtLastAsync();
            return;
        }
        throw new CustomBasicException("Should not be trading cards");
    }
    public async Task PlayCardAsync(LifeCardGameCardInformation thisCard)
    {
        if (OtherTurn > 0)
        {
            throw new CustomBasicException("Can't play a card when its otherturn.  Try creating a routine to process the otherturn");
        }
        if (thisCard.Points == 0)
        {
            throw new CustomBasicException("Must have at least 5 points");
        }
        SingleInfo!.MainHandList.RemoveObjectByDeck(thisCard.Deck);
        _model!.CurrentPile!.AddCard(thisCard);
        if (thisCard.OpponentKeepsCard == false)
        {
            SingleInfo.LifeStory!.AddCard(thisCard);
            _gameContainer!.PlayerChosen = WhoTurn;
            _gameContainer.CardChosen = thisCard;
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
            SingleInfo.Points = SingleInfo.LifeStory.TotalPoints();
            _command.UpdateAll();
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(1);
            }
            _gameContainer.PlayerChosen = 0;
            _gameContainer.CardChosen = null;
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
            if (thisCard.Action == EnumAction.TurnBackTime)
            {
                await TurnBackTimeAsync();
                return;
            }
            if (thisCard.Action == EnumAction.LifeSwap)
            {
                await ContinueTurnAsync();
                return;
            }
            _model.CurrentPile.ClearCards();
            await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
            await DrawAtLastAsync();
            return;
        }
        if (thisCard.Action == EnumAction.None)
        {
            throw new CustomBasicException("Must have an action card currently if an opponent is keeping this card");
        }
        await PlayerList!.RepositionCardsAsync(this, _gameContainer, _model);
        await ContinueTurnAsync();
    }
    public bool CanPlayCard(LifeCardGameCardInformation thisCard)
    {
        if (thisCard.OnlyOneAllowed())
        {
            return !SingleInfo!.LifeStory!.HandList.Any(items => items.SpecialCategory == thisCard.SpecialCategory);
        }
        if (thisCard.Requirement != EnumSpecialCardCategory.None && thisCard.Action != EnumAction.MovingHouse)
        {
            return SingleInfo!.LifeStory!.HandList.Any(items => items.SpecialCategory == thisCard.Requirement);
        }
        if (thisCard.IsPayday())
        {
            var careers = SingleInfo!.LifeStory!.HandList.Count(items => items.SwitchCategory == EnumSwitchCategory.Career);
            var pays = SingleInfo.LifeStory.HandList.Count(items => items.IsPayday());
            var maxs = careers * 3;
            var actual = pays + 1;
            return actual <= maxs;
        }
        if (thisCard.Action == EnumAction.TurnBackTime)
        {
            return SaveRoot!.YearList.Count > 0;
        }
        if (thisCard.Action == EnumAction.None || thisCard.Action == EnumAction.LifeSwap || thisCard.Action == EnumAction.MidlifeCrisis)
        {
            return true;
        }
        var opponentList = PlayerList!.OpponentStory();
        if (thisCard.Action == EnumAction.AdoptBaby)
        {
            return opponentList.Any(items => items.SwitchCategory == EnumSwitchCategory.Baby);
        }
        if (thisCard.Action == EnumAction.CareerSwap)
        {
            if (SingleInfo!.LifeStory!.HandList.Any(items => items.SwitchCategory == EnumSwitchCategory.Career) == false)
            {
                return false;
            }
            return opponentList.Any(items => items.SwitchCategory == EnumSwitchCategory.Career);
        }
        if (thisCard.Action == EnumAction.DonateToCharity)
        {
            return opponentList.Any(items => items.Category == EnumFirstCardCategory.Wealth && items.Points > 5 && items.SpecialCategory != EnumSpecialCardCategory.Passport);
        }
        if (thisCard.Action == EnumAction.IMTheBoss || thisCard.Action == EnumAction.YoureFired)
        {
            return opponentList.Any(items => items.IsPayday());
        }
        if (thisCard.Action == EnumAction.Lawsuit)
        {
            return opponentList.Any(items => items.Points >= 30 && items.SpecialCategory != EnumSpecialCardCategory.Marriage);
        }
        if (thisCard.Action == EnumAction.LongLostRelative)
        {
            return opponentList.Any(items => items.Category == EnumFirstCardCategory.Family && items.SpecialCategory != EnumSpecialCardCategory.Marriage && items.Points > 5);
        }
        if (thisCard.Action == EnumAction.LostPassport)
        {
            return opponentList.Any(items => items.SpecialCategory == EnumSpecialCardCategory.Passport);
        }
        if (thisCard.Action == EnumAction.MixUpAtVets)
        {
            if (SingleInfo!.LifeStory!.HandList.Any(items => items.SwitchCategory == EnumSwitchCategory.Pet) == false)
            {
                return false;
            }
            return opponentList.Any(items => items.SwitchCategory == EnumSwitchCategory.Pet);
        }
        if (thisCard.Action == EnumAction.MovingHouse)
        {
            if (SingleInfo!.LifeStory!.HandList.Any(items => items.SpecialCategory == EnumSpecialCardCategory.House) == false)
            {
                return false;
            }
            return opponentList.Any(items => items.SpecialCategory == EnumSpecialCardCategory.House);
        }
        if (thisCard.Action == EnumAction.SecondChance)
        {
            return opponentList.Any(items => items.Points >= 10 && items.Points <= 30 && items.SpecialCategory != EnumSpecialCardCategory.Passport);
        }
        if (thisCard.Action == EnumAction.YourStory)
        {
            return opponentList.Any(items => items.Category == EnumFirstCardCategory.Adventure && items.Points != 5);
        }
        throw new CustomBasicException("Don't know if it can play the card or not?");
    }
}