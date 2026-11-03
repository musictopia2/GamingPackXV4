namespace GoFish.Core.Logic;
[SingletonGame]
public class GoFishMainGameClass(IGamePackageResolver mainContainer,
    IEventAggregator aggregator,
    BasicData basicData,
    TestOptions test,
    GoFishVMData currentMod,
    IMultiplayerSaveState state,
    IAsyncDelayer delay,
    ICardInfo<RegularSimpleCard> cardInfo,
    CommandContainer command,
    GoFishGameContainer gameContainer,
    IAskProcesses processes,
    ISystemError error,
    IToast toast
        )
        : CardGameClass<RegularSimpleCard, GoFishPlayerItem, GoFishSaveInfo>(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    , IMiscDataNM, IFinishStart, ISerializable
{
    public bool IsValidMove(DeckRegularDict<RegularSimpleCard> thisCol)
    {
        if (Test!.AllowAnyMove == true)
        {
            return true;
        }
        return thisCol.First().Value == thisCol.Last().Value;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        return base.FinishGetSavedAsync();
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
            await Delay!.DelaySeconds(.5);
        }
        if (SaveRoot!.RemovePairs == false && SaveRoot.NumberAsked == false)
        {
            if (PlayerList.Count == 2)
            {
                await processes.NumberToAskAsync(cc1.NumberToAsk(SaveRoot));
                return;
            }
            throw new CustomBasicException("Only 2 players are supported now");
        }
        var thisList = cc1.PairToPlay(SaveRoot);
        if (thisList.Count == 0)
        {
            await EndTurnAsync();
            return;
        }
        if (thisList.Count != 2)
        {
            throw new CustomBasicException("Needed one pair to remove.  Rethink");
        }
        await ProcessPlayAsync(thisList.First().Deck, thisList.Last().Deck);
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        SaveRoot!.RemovePairs = true;
        SaveRoot.NumberAsked = false;
        currentMod.AskList.ItemList.Clear();
        PlayerList!.ForEach(items => items.Pairs = 0);
        return base.StartSetUpAsync(isBeginning);
    }
    public async Task ProcessPlayAsync(int deck1, int deck2)
    {
        SingleInfo!.MainHandList.RemoveObjectByDeck(deck1);
        RegularSimpleCard secondCard = SingleInfo.MainHandList.RemoveObjectByDeck(deck2);
        SingleInfo.Pairs++;
        secondCard.IsSelected = false;
        secondCard.Drew = false;
        if (currentMod!.Pile1!.CardExist(secondCard.Deck) == false)
        {
            currentMod.Pile1.AddCard(secondCard);
        }
        else if (currentMod.Pile1.CardExist(deck2) == false)
        {
            secondCard = SingleInfo.MainHandList.RemoveObjectByDeck(deck1);
            currentMod.Pile1.AddCard(secondCard);
        }
        if (SingleInfo.MainHandList.Count == 0)
        {
            int cards = currentMod.Deck1!.CardsLeft();
            if (cards < 5 && cards > 0)
            {
                LeftToDraw = cards;
                PlayerDraws = WhoTurn;
                await DrawAsync();
                processes.LoadAskList();
                return;
            }
            else if (cards > 0)
            {
                LeftToDraw = 5;
                PlayerDraws = WhoTurn;
                await DrawAsync();
                processes.LoadAskList();
            }
        }
        await ContinueTurnAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "numbertoask":
                EnumRegularCardValueList thisValue = await js1.DeserializeObjectAsync<EnumRegularCardValueList>(content);
                await processes.NumberToAskAsync(thisValue);
                return;
            case "processplay":
                SendPair thisPair = await js1.DeserializeObjectAsync<SendPair>(content);
                await ProcessPlayAsync(thisPair.Card1, thisPair.Card2);
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    private bool CanEndGame()
    {
        if (Test!.ImmediatelyEndGame)
        {
            return true;
        }
        foreach (var thisPlayer in PlayerList!)
        {
            if (thisPlayer.MainHandList.Count != 0)
            {
                return false;
            }
        }
        return true;
    }
    private async Task GameOverAsync()
    {
        SingleInfo = PlayerList.OrderByDescending(items => items.Pairs).Take(1).Single();
        if (SingleInfo.Pairs == 13)
        {
            await ShowTieAsync();
        }
        else
        {
            await ShowWinAsync();
        }
    }

    public override async Task EndTurnAsync()
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            currentMod!.PlayerHand1!.EndTurn();
        }
        if (CanEndGame() == true)
        {
            await GameOverAsync();
            return;
        }
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        if (SaveRoot!.RemovePairs == true && WhoTurn == WhoStarts)
        {
            SaveRoot.RemovePairs = false;
        }
        await StartNewTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        SaveRoot!.NumberAsked = false;
        if (SaveRoot.RemovePairs == false)
        {
            processes.LoadAskList();
        }
        await ContinueTurnAsync();
    }
    public Task FinishStartAsync()
    {
        if (SaveRoot!.RemovePairs == false)
        {
            processes.LoadAskList();
        }
        return Task.CompletedTask;
    }
}