namespace HorseshoeCardGame.Core.Logic;
[SingletonGame]
public class HorseshoeTrickAreaCP(HorseshoeCardGameGameContainer gameContainer) : PossibleDummyTrickObservable<EnumSuitList, HorseshoeCardGameCardInformation,
    HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo>(gameContainer), ITrickPlay, IAdvancedTrickProcesses
{
    protected override bool UseDummy { get; set; } = true;
    protected override int GetCardIndex()
    {
        var thisSingle = ViewList!.Where(items => items.Player == gameContainer.WhoTurn && items.PossibleDummy == gameContainer.SaveRoot!.FirstCardPlayed).Single();
        return ViewList!.IndexOf(thisSingle);
    }
    protected override void PopulateNewCard(HorseshoeCardGameCardInformation oldCard, ref HorseshoeCardGameCardInformation newCard) { }
    protected override void PopulateOldCard(HorseshoeCardGameCardInformation oldCard) { }
    protected override string FirstHumanText()
    {
        return "Your First:";
    }
    protected override string DummyHumanText()
    {
        return "Your Last:";
    }
    protected override string FirstOpponentText()
    {
        return "Opponent First:";
    }
    protected override string DummyOpponentText()
    {
        return "Opponent Last:";
    }
    protected override async Task ProcessCardClickAsync(HorseshoeCardGameCardInformation thisCard)
    {
        int index = CardList.IndexOf(thisCard);
        if (index == 0 && gameContainer.SaveRoot!.FirstCardPlayed == false)
        {
            await gameContainer.CardClickedAsync!.Invoke();
            return;
        }
        if (index == 3 && gameContainer.SaveRoot!.FirstCardPlayed == true)
        {
            await gameContainer.CardClickedAsync!.Invoke();
            return;
        }
    }
    public async Task AnimateWinAsync(int wins)
    {
        WinCard = GetWinningCard(wins);
        await AnimateWinAsync();
    }
    private HorseshoeCardGameCardInformation GetWinningCard(int wins)
    {
        var tempList = OrderList.ToRegularDeckDict();
        return tempList.First(Items => Items.Player == wins);
    }
    protected override int GetMaxCount()
    {
        return 4;
    }
    internal bool DidPlay2Cards => gameContainer.SaveRoot!.TrickList.Count == 2;
    public void ClearBoard()
    {
        DeckRegularDict<HorseshoeCardGameCardInformation> tempList = [];
        for (int x = 1; x <= 4; x++)
        {
            HorseshoeCardGameCardInformation thisCard = new();
            thisCard.Populate(x);
            thisCard.Deck += 1000; //this was the workaround.
            thisCard.IsUnknown = true;
            thisCard.Visible = true;
            tempList.Add(thisCard);
        }
        OrderList.Clear();
        CardList.ReplaceRange(tempList);
        Visible = true;
    }
    public void LoadGame()
    {
        var tempList = OrderList.ToRegularDeckDict();
        ClearBoard();
        if (tempList.Count == 0)
        {
            return;
        }
        int index;
        int tempTurn;
        bool previousFirst;
        previousFirst = gameContainer.SaveRoot!.FirstCardPlayed;
        HorseshoeCardGameCardInformation lastCard;
        tempTurn = gameContainer.WhoTurn;
        DeckRegularDict<HorseshoeCardGameCardInformation> otherList = [];
        int x = 0;
        tempList.ForEach(thisCard =>
        {
            if (thisCard.Player == 0)
            {
                throw new CustomBasicException("The Player Cannot Be 0");
            }
            gameContainer.WhoTurn = thisCard.Player;
            gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
            x++;
            if (x == 1 || x == 2)
            {
                gameContainer.SaveRoot.FirstCardPlayed = false;
            }
            else
            {
                gameContainer.SaveRoot.FirstCardPlayed = true;
            }
            index = GetCardIndex();
            lastCard = gameContainer.GetBrandNewCard(thisCard.Deck);
            lastCard.Player = thisCard.Player;
            TradeCard(index, lastCard);
            otherList.Add(lastCard);
        });
        OrderList.ReplaceRange(otherList);
        gameContainer.SaveRoot.FirstCardPlayed = previousFirst;
        gameContainer.WhoTurn = tempTurn;
    }
}